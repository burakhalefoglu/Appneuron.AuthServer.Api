using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.UserClaims.Commands;
using Business.Handlers.UserClaims.Queries;
using Business.Internals.Handlers.UserClaims;
using Business.Internals.Handlers.Users;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserClaims.Commands.CreateUserClaimCommand;
using static Business.Handlers.UserClaims.Commands.DeleteUserClaimCommand;
using static Business.Handlers.UserClaims.Commands.UpdateUserClaimCommand;
using static Business.Handlers.UserClaims.Queries.GetUserClaimsQuery;
using static Business.Internals.Handlers.UserClaims.CreateUserClaimsInternalCommand;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UserClaimHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _userClaimRepository = new Mock<IUserClaimRepository>();
            _createUserClaimCommandHandler =
                new CreateUserClaimCommandHandler(_userClaimRepository.Object);
            _deleteUserClaimCommandHandler = new DeleteUserClaimCommandHandler(_userClaimRepository.Object);
            _updateUserClaimCommandHandler =
                new UpdateUserClaimCommandHandler(_userClaimRepository.Object, _mediator.Object);
            _getUserClaimsQueryHandler = new GetUserClaimsQueryHandler(_userClaimRepository.Object);
            _createUserClaimsInternalCommandHandler = new CreateUserClaimsInternalCommandHandler(_userClaimRepository.Object);
        }

        private Mock<IMediator> _mediator;
        private Mock<IUserClaimRepository> _userClaimRepository;
        private CreateUserClaimCommandHandler _createUserClaimCommandHandler;
        private DeleteUserClaimCommandHandler _deleteUserClaimCommandHandler;
        private UpdateUserClaimCommandHandler _updateUserClaimCommandHandler;
        private GetUserClaimsQueryHandler _getUserClaimsQueryHandler;
        private CreateUserClaimsInternalCommandHandler _createUserClaimsInternalCommandHandler;
       
        [Test]
        public async Task UserClaim_CreateUserClaim_Success()
        {
            var query = new CreateUserClaimCommand
            {
                ClaimId = 1,
                UserId = 1
            };

            _userClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .ReturnsAsync((UserClaim) null);
            _userClaimRepository.Setup(x => x.Add(It.IsAny<UserClaim>()));
            var result = await _createUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task UserClaim_CreateUserClaimInternalCommand_Success()
        {
            var query = new CreateUserClaimsInternalCommand()
            {
                UserId = 1,
                OperationClaims = new List<OperationClaim>()
            };

            _userClaimRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<UserClaim, bool>>>()))
                .ReturnsAsync(true);
            _userClaimRepository.Setup(x => x.Add(It.IsAny<UserClaim>()));
            var result = await _createUserClaimsInternalCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }
        
        [Test]
        public async Task UserClaim_CreateUserClaim_UserClaimExit()
        {
            var query = new CreateUserClaimCommand
            {
                ClaimId = 1,
                UserId = 1
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
        public async Task UserClaim_DeleteUserClaim_Deleted()
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
        public async Task UserClaim_UpdateUserClaim_Success()
        {
            var query = new UpdateUserClaimCommand
            {
                ClaimId = new long[]
                {
                    1,2,3
                },
                Id = 1
            };
            _mediator.Setup(x => x.Send(It.IsAny<GetUserInternalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<User>(new User()));
            foreach (var l in query.ClaimId)
            {
                _userClaimRepository.Setup(x =>
                    x.AddAsync(It.IsAny<UserClaim>()));
            }
            var result = await _updateUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        
        [Test]
        public async Task UserClaim_UpdateUserClaim_UserNotFound()
        {
            var query = new UpdateUserClaimCommand
            {
                ClaimId = new long[]
                {
                   1,2,3
                },
                Id = 1
            };
            _mediator.Setup(x => x.Send(It.IsAny<GetUserInternalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<User>((User) null));
            foreach (var l in query.ClaimId)
            {
                _userClaimRepository.Setup(x =>
                    x.AddAsync(It.IsAny<UserClaim>()));
            }
            var result = await _updateUserClaimCommandHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserNotFound);
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