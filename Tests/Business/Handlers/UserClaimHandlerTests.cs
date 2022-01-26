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
using DataAccess.Abstract;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserClaims.Commands.CreateUserClaimCommand;
using static Business.Handlers.UserClaims.Commands.DeleteUserClaimCommand;
using static Business.Handlers.UserClaims.Commands.UpdateUserClaimCommand;
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
            _createUserClaimCommandHandler =
                new CreateUserClaimCommandHandler(_userClaimRepository.Object);
            _deleteUserClaimCommandHandler = new DeleteUserClaimCommandHandler(_userClaimRepository.Object);
            _updateUserClaimCommandHandler =
                new UpdateUserClaimCommandHandler(_userClaimRepository.Object);
            _getUserClaimsQueryHandler = new GetUserClaimsQueryHandler(_userClaimRepository.Object);
        }

        private Mock<IUserClaimRepository> _userClaimRepository;
        private CreateUserClaimCommandHandler _createUserClaimCommandHandler;
        private DeleteUserClaimCommandHandler _deleteUserClaimCommandHandler;
        private UpdateUserClaimCommandHandler _updateUserClaimCommandHandler;
        private GetUserClaimsQueryHandler _getUserClaimsQueryHandler;

        [Test]
        public async Task UserClaim_CreateUserClaim_Success()
        {
            var query = new CreateUserClaimCommand
            {
                ClaimId = "test_claim_ıd",
                UserId = "test_user_ıd"
            };

            _userClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .ReturnsAsync((UserClaim) null);
            _userClaimRepository.Setup(x => x.Add(It.IsAny<UserClaim>()));
            var result = await _createUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task UserClaim_CreateUserClaim_UserClaimExit()
        {
            var query = new CreateUserClaimCommand
            {
                ClaimId = "test_claim_ıd",
                UserId = "test_user_ıd"
            };

            _userClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .ReturnsAsync(new UserClaim());

            _userClaimRepository.Setup(x => x.Add(It.IsAny<UserClaim>()));

            var result = await _createUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserClaimExit);
        }


        [Test]
        public async Task UserClaim_DeleteUserClaim_UserClaimNotFound()
        {
            var query = new DeleteUserClaimCommand
            {
                Id = "test_user_ıd"
            };

            _userClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .Returns(Task.FromResult<UserClaim>(null));

            var result = await _deleteUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserClaimNotFound);
        }

        [Test]
        public async Task UserClaim_DeleteUserClaim_Deleted()
        {
            var query = new DeleteUserClaimCommand
            {
                Id = "test_user_ıd"
            };

            _userClaimRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .Returns(Task.FromResult(new UserClaim()));

            var result = await _deleteUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task UserClaim_UpdateUserClaim_Success()
        {
            var query = new UpdateUserClaimCommand
            {
                ClaimId = new[]
                {
                    "test_claim_ıd", "test_claim_ıd2", "test_claim_ıd3"
                },
                Id = "test_user_ıd"
            };

            _userClaimRepository.Setup(x =>
                x.AddManyAsync(It.IsAny<IEnumerable<UserClaim>>()));

            var result = await _updateUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task UserClaim_GetUserClaims_Success()
        {
            var query = new GetUserClaimsQuery();

            _userClaimRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .ReturnsAsync(new List<UserClaim>
                {
                    new(),
                    new(),
                    new()
                }.AsQueryable());

            var result = await _getUserClaimsQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
        }
    }
}