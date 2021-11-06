using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Fakes.Handlers.GroupClaims;
using Business.Fakes.Handlers.UserProjects;
using Business.Handlers.Clients.Commands;
using Business.Services.Authentication;
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
    public class ClientTests
    {
        private Mock<ITokenHelper> _tokenHelper;
        private Mock<IClientRepository> _clientRepository;
        private Mock<IMediator> _mediator;

        private CreateTokenCommandHandler _createTokenCommandHandler;

        [SetUp]
        public void Setup()
        {
            _clientRepository = new Mock<IClientRepository>();
            _tokenHelper = new Mock<ITokenHelper>();
            _mediator = new Mock<IMediator>();

            _createTokenCommandHandler = new CreateTokenCommandHandler(_clientRepository.Object,
                _tokenHelper.Object, _mediator.Object);
        }


        [Test]
        public async Task Handler_CreateTokenCommand_ProjectNotFound()
        {
            var command = new CreateTokenCommand
            {
                ClientId = "dasdas",
                ProjectId = "wqweqwe"
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetUserProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<UserProject>());

            var result = await _createTokenCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.ProjectNotFound);
        }


        [Test]
        public async Task Handler_CreateTokenCommand_SuccessfulLoginWhenNewClientRequest()
        {
            var command = new CreateTokenCommand
            {
                ClientId = "dasdas",
                ProjectId = "wqweqwe"
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetUserProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<UserProject>(new UserProject()
                {
                    ProjectKey = "sadasd",
                    Id = 1,
                    UserId = 10
                }));

            _clientRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult<Client>(null));

            _clientRepository.Setup(x => x.Add(It.IsAny<Client>()))
                .Returns(new Client()
                {
                    ClientId = "ljhjhdfhft",
                    Id = 2,
                    ProjectId = "zffcsffcas"
                });

            _mediator.Setup(x => x.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IDataResult<IEnumerable<SelectionItem>>>
                    (new SuccessDataResult<IEnumerable<SelectionItem>>(
                        new List<SelectionItem>()
                        {
                            new SelectionItem()
                            {
                                Id = 1,
                                IsDisabled = false,
                                Label = "Test",
                                ParentId = "dfsa"
                            }
                        })));
            _tokenHelper.Setup(x => x.CreateClientToken<DArchToken>(It.IsAny<ClientClaimModel>()))
                .Returns(new DArchToken());

            var result = await _createTokenCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.SuccessfulLogin);
        }


        [Test]
        public async Task Handler_CreateTokenCommand_SuccessfulLoginWhenOldClientRequest()
        {
            var command = new CreateTokenCommand
            {
                ClientId = "dasdas",
                ProjectId = "wqweqwe"
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetUserProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<UserProject>(new UserProject()
                {
                    ProjectKey = "sadasd",
                    Id = 1,
                    UserId = 10
                }));

            _clientRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult(new Client()
                {
                    ClientId = "ljhjhdfhft",
                    Id = 2,
                    ProjectId = "zffcsffcas"
                }));

            _mediator.Setup(x => x.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IDataResult<IEnumerable<SelectionItem>>>
                    (new SuccessDataResult<IEnumerable<SelectionItem>>(
                        new List<SelectionItem>()
                        {
                            new SelectionItem()
                            {
                                Id = 1,
                                IsDisabled = false,
                                Label = "Test",
                                ParentId = "dfsa"
                            }
                        })));
            _tokenHelper.Setup(x => x.CreateClientToken<DArchToken>(It.IsAny<ClientClaimModel>()))
                .Returns(new DArchToken());

            var result = await _createTokenCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.SuccessfulLogin);
        }


    }
}