using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.UserClaims.Commands;
using Business.Handlers.UserClaims.Queries;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserClaims.Commands.CreateUserClaimCommand;
using static Business.Handlers.UserClaims.Commands.DeleteUserClaimCommand;
using static Business.Handlers.UserClaims.Commands.UpdateUserClaimCommand;
using static Business.Handlers.UserClaims.Queries.GetUserClaimLookupByUserIdQuery;
using static Business.Handlers.UserClaims.Queries.GetUserClaimLookupQuery;
using static Business.Handlers.UserClaims.Queries.GetUserClaimsQuery;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UserClaimHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _userClaimRepository = new Mock<IUserClaimRepository>();
            _mediator = new Mock<IMediator>();

            _createUserClaimCommandHandler =
                new CreateUserClaimCommandHandler(_userClaimRepository.Object);
            _deleteUserClaimCommandHandler = new DeleteUserClaimCommandHandler(_userClaimRepository.Object);
            _updateUserClaimCommandHandler =
                new UpdateUserClaimCommandHandler(_userClaimRepository.Object);
            _getUserClaimLookupByUserIdQueryHandler =
                new GetUserClaimLookupByUserIdQueryHandler(_userClaimRepository.Object, _mediator.Object);
            _getUserClaimLookupQueryHandler = new GetUserClaimLookupQueryHandler(_userClaimRepository.Object);
            _getUserClaimsQueryHandler = new GetUserClaimsQueryHandler(_userClaimRepository.Object);
        }

        private Mock<IUserClaimRepository> _userClaimRepository;
        private Mock<IMediator> _mediator;

        private CreateUserClaimCommandHandler _createUserClaimCommandHandler;
        private DeleteUserClaimCommandHandler _deleteUserClaimCommandHandler;
        private UpdateUserClaimCommandHandler _updateUserClaimCommandHandler;
        private GetUserClaimLookupByUserIdQueryHandler _getUserClaimLookupByUserIdQueryHandler;
        private GetUserClaimLookupQueryHandler _getUserClaimLookupQueryHandler;
        private GetUserClaimsQueryHandler _getUserClaimsQueryHandler;

        [Test]
        public async Task Handler_CreateUserClaim_Success()
        {
            var query = new CreateUserClaimCommand
            {
                ClaimId = 1,
                UserId = 2
            };

            _userClaimRepository.Setup(x => x.Add(It.IsAny<UserClaim>()));

            var result = await _createUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Handler_DeleteUserClaim_UserClaimNotFound()
        {
            var query = new DeleteUserClaimCommand
            {
                Id = 1
            };

            _userClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .Returns(Task.FromResult<UserClaim>(null));

            var result = await _deleteUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserClaimNotFound);
        }

        [Test]
        public async Task Handler_DeleteUserClaim_Deleted()
        {
            var query = new DeleteUserClaimCommand
            {
                Id = 1
            };

            _userClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .Returns(Task.FromResult(new UserClaim()));

            var result = await _deleteUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task Handler_UpdateUserClaim_Success()
        {
            var query = new UpdateUserClaimCommand
            {
                ClaimId = new int[3]
                {
                    1, 2, 3
                },
                Id = 1
            };

            _userClaimRepository.Setup(x =>
                x.BulkInsert(It.IsAny<int>(),
                    It.IsAny<IEnumerable<UserClaim>>()));

            var result = await _updateUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Handler_GetUserClaimLookupByUserId_Success()
        {
            var query = new GetUserClaimLookupByUserIdQuery
            {
                Id = 1
            };

            _userClaimRepository.Setup(x => x.GetUserClaimSelectedList(It.IsAny<int>()))
                .Returns(Task.FromResult<IEnumerable<SelectionItem>>(new List<SelectionItem>
                {
                    new(),
                    new()
                }));

            var result = await _getUserClaimLookupByUserIdQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Handler_GetUserClaimLookup_Success()
        {
            var query = new GetUserClaimLookupQuery
            {
                UserId = 1
            };

            _userClaimRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<UserClaim>>(new List<UserClaim>
                {
                    new(),
                    new(),
                    new()
                }));

            var result = await _getUserClaimLookupQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Handler_GetUserClaims_Success()
        {
            var query = new GetUserClaimsQuery();

            _userClaimRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<UserClaim>>(new List<UserClaim>
                {
                    new(),
                    new(),
                    new()
                }));

            var result = await _getUserClaimsQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
        }
    }
}