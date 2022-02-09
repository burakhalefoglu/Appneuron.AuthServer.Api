using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.GroupClaims.Commands;
using Business.Handlers.GroupClaims.Queries;
using Business.Internals.Handlers.GroupClaims;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.GroupClaims.Commands.CreateGroupClaimCommand;
using static Business.Handlers.GroupClaims.Commands.DeleteGroupClaimCommand;
using static Business.Handlers.GroupClaims.Commands.UpdateGroupClaimCommand;
using static Business.Handlers.GroupClaims.Queries.GetGroupClaimQuery;
using static Business.Internals.Handlers.GroupClaims.GetGroupClaimsLookupByGroupIdInternalQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class GroupClaimHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _groupClaimRepository = new Mock<IGroupClaimRepository>();
            _operationClaimRepository = new Mock<IOperationClaimRepository>();

            _createGroupClaimCommandHandler =
                new CreateGroupClaimCommandHandler(_groupClaimRepository.Object, _operationClaimRepository.Object);
            _deleteGroupClaimCommandHandler = new DeleteGroupClaimCommandHandler(_groupClaimRepository.Object);
            _updateGroupClaimCommandHandler =
                new UpdateGroupClaimCommandHandler(_groupClaimRepository.Object, _operationClaimRepository.Object);

            _getGroupClaimQueryHandler = new GetGroupClaimQueryHandler(_groupClaimRepository.Object);
            _getGroupClaimsLookupByGroupIdInternalQueryHandler =
                new GetGroupClaimsLookupByGroupIdInternalQueryHandler(_groupClaimRepository.Object, _mediator.Object);
        }

        private Mock<IMediator> _mediator;

        private Mock<IGroupClaimRepository> _groupClaimRepository;
        private Mock<IOperationClaimRepository> _operationClaimRepository;

        private CreateGroupClaimCommandHandler _createGroupClaimCommandHandler;
        private DeleteGroupClaimCommandHandler _deleteGroupClaimCommandHandler;
        private UpdateGroupClaimCommandHandler _updateGroupClaimCommandHandler;

        private GetGroupClaimQueryHandler _getGroupClaimQueryHandler;
        private GetGroupClaimsLookupByGroupIdInternalQueryHandler _getGroupClaimsLookupByGroupIdInternalQueryHandler;
        [Test]
        public async Task GroupClaim_CreateGroupClaim_OperationClaimNotFound()
        {
            var command = new CreateGroupClaimCommand
            {
                ClaimName = "test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<OperationClaim>(null));


            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult<GroupClaim>(null));

            _operationClaimRepository.Setup(x => x.Add(It.IsAny<OperationClaim>()));

            var result = await _createGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.OperationClaimNotFound);
        }

        [Test]
        public async Task GroupClaim_CreateGroupClaim_GroupClaimExit()
        {
            var command = new CreateGroupClaimCommand
            {
                ClaimName = "test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult(new OperationClaim()));


            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim()));

            _operationClaimRepository.Setup(x => x.Add(It.IsAny<OperationClaim>()));

            var result = await _createGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.GroupClaimExit);
        }

        [Test]
        public async Task GroupClaim_CreateGroupClaim_Added()
        {
            var command = new CreateGroupClaimCommand
            {
                ClaimName = "test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult(new OperationClaim()));


            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult<GroupClaim>(null));

            _operationClaimRepository.Setup(x => x.Add(It.IsAny<OperationClaim>()));

            var result = await _createGroupClaimCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }


        [Test]
        public async Task GroupClaim_UpdateGroupClaim_Updated()
        {
            var command = new UpdateGroupClaimCommand
            {
                GroupId = 1,
                Id = 1,
                ClaimId = 1,
                ClaimName = "Test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .ReturnsAsync(new OperationClaim());

            _groupClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .ReturnsAsync(new GroupClaim());

            _groupClaimRepository.Setup(x => x.Add(
                It.IsAny<GroupClaim>()));

            var result = await _updateGroupClaimCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task GroupClaim_UpdateGroupClaim_OperationClaimNotFound()
        {
            var command = new UpdateGroupClaimCommand
            {
                GroupId = 1,
                Id = 1,
                ClaimId = 1,
                ClaimName = "Test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .ReturnsAsync((OperationClaim) null);

            _groupClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .ReturnsAsync(new GroupClaim());

            _groupClaimRepository.Setup(x => x.Add(
                It.IsAny<GroupClaim>()));

            var result = await _updateGroupClaimCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.OperationClaimNotFound);
        }


        [Test]
        public async Task GroupClaim_UpdateGroupClaim_GroupClaimNotFound()
        {
            var command = new UpdateGroupClaimCommand
            {
                GroupId = 1,
                Id = 1,
                ClaimId = 1,
                ClaimName = "Test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .ReturnsAsync(new OperationClaim());

            _groupClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .ReturnsAsync((GroupClaim) null);

            _groupClaimRepository.Setup(x => x.Add(
                It.IsAny<GroupClaim>()));

            var result = await _updateGroupClaimCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.GroupClaimNotFound);
        }


        [Test]
        public async Task GroupClaim_DeleteGroupClaim_GroupClaimNotFound()
        {
            var command = new DeleteGroupClaimCommand
            {
                Id = 1,
            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult<GroupClaim>(null));

            var result = await _deleteGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.GroupClaimNotFound);
        }

        [Test]
        public async Task GroupClaim_DeleteGroupClaim_Deleted()
        {
            var command = new DeleteGroupClaimCommand
            {
                Id = 1,
            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim
                {
                    GroupId = 1,
                    Id = 1,
                    ClaimId = 1,
                }));

            _groupClaimRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<GroupClaim>()));

            var result = await _deleteGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task GroupClaim_GetGroupClaim_Success()
        {
            var command = new GetGroupClaimQuery
            {
                Id = 1,
            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim
                {
                    GroupId = 1,
                    Id = 1,
                    ClaimId = 1,
                }));

            var result = await _getGroupClaimQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.ClaimId.Should().Be(1);
        }
        
        
        [Test]
        public async Task GroupClaim_GetGroupClaimsLookupByGroupIdInternal_Success()
        {
            var command = new GetGroupClaimsLookupByGroupIdInternalQuery
            {
                GroupId = 1,
                
            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim
                {
                    GroupId = 1,
                    Id = 1,
                    ClaimId = 1,
                }));

            var result = await _getGroupClaimsLookupByGroupIdInternalQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
        }
    }
}