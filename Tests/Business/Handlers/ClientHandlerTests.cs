﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Fakes.Handlers.UserProjects;
using Business.Handlers.Clients.Commands;
using Business.Internals.Handlers.GroupClaims;
using Business.MessageBrokers;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Clients.Commands.CreateTokenCommand;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class ClientHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _clientRepository = new Mock<IClientRepository>();
            _tokenHelper = new Mock<ITokenHelper>();
            _mediator = new Mock<IMediator>();
            _messageBroker = new Mock<IMessageBroker>();

            _createTokenCommandHandler = new CreateTokenCommandHandler(_clientRepository.Object,
                _tokenHelper.Object, _mediator.Object, _messageBroker.Object);
        }

        private Mock<ITokenHelper> _tokenHelper;
        private Mock<IClientRepository> _clientRepository;
        private Mock<IMediator> _mediator;
        private Mock<IMessageBroker> _messageBroker;


        private CreateTokenCommandHandler _createTokenCommandHandler;


        [Test]
        public async Task Client_CreateTokenCommand_ProjectNotFound()
        {
            var command = new CreateTokenCommand
            {
                ClientId = "test_client_ıd",
                ProjectId = "test_project_ıd"
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetUserProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<UserProject>());

            var result = await _createTokenCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.ProjectNotFound);
        }


        [Test]
        public async Task Client_CreateTokenCommand_SuccessfulLoginWhenNewClientRequest()
        {
            var command = new CreateTokenCommand
            {
                ClientId = "test_client_ıd",
                ProjectId = "test_project_ıd"
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetUserProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<UserProject>(new UserProject
                {
                    ProjectKey = "test_project_key",
                    UserId = "test_user_ıd"
                }));

            _clientRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult<Client>(null));

            _clientRepository.Setup(x => x.Add(It.IsAny<Client>()));
            _mediator.Setup(x => x.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IDataResult<IEnumerable<SelectionItem>>>
                (new SuccessDataResult<IEnumerable<SelectionItem>>(
                    new List<SelectionItem>
                    {
                        new()
                        {
                            Id = "507f1f77bcf86cd799439011",
                            IsDisabled = false,
                            Label = "Test",
                            ParentId = "test_parent_ıd"
                        }
                    })));
            _tokenHelper.Setup(x => x.CreateClientToken<AccessToken>(It.IsAny<ClientClaimModel>()))
                .Returns(new AccessToken());

            var result = await _createTokenCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.SuccessfulLogin);
        }


        [Test]
        public async Task Client_CreateTokenCommand_SuccessfulLoginWhenOldClientRequest()
        {
            var command = new CreateTokenCommand
            {
                ClientId = "test_client_ıd",
                ProjectId = "test_project_ıd"
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetUserProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<UserProject>(new UserProject
                {
                    ProjectKey = "test_project_ıd",
                    UserId = "test_user_ıd"
                }));

            _clientRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult(new Client
                {
                    ProjectId = "test_project_ıd"
                }));

            _mediator.Setup(x => x.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IDataResult<IEnumerable<SelectionItem>>>
                (new SuccessDataResult<IEnumerable<SelectionItem>>(
                    new List<SelectionItem>
                    {
                        new()
                        {
                            Id = "507f1f77bcf86cd799439011",
                            IsDisabled = false,
                            Label = "Test",
                            ParentId = "test_ıd"
                        }
                    })));
            _tokenHelper.Setup(x => x.CreateClientToken<AccessToken>(It.IsAny<ClientClaimModel>()))
                .Returns(new AccessToken());

            var result = await _createTokenCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.SuccessfulLogin);
        }
    }
}