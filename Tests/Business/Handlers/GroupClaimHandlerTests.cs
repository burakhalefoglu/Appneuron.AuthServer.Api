using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.GroupClaims.Commands;
using Business.Handlers.GroupClaims.Queries;
using Business.Handlers.Groups.Queries;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Business.Handlers.GroupClaims.Commands.CreateGroupClaimCommand;
using static Business.Handlers.GroupClaims.Commands.DeleteGroupClaimCommand;
using static Business.Handlers.GroupClaims.Commands.UpdateGroupClaimCommand;
using static Business.Handlers.GroupClaims.Queries.GetGroupClaimQuery;
using static Business.Handlers.GroupClaims.Queries.GetGroupClaimsLookupByGroupIdQuery;
using static Business.Handlers.GroupClaims.Queries.GetGroupClaimsQuery;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class GroupClaimHandlerTests
    {
        private Mock<IGroupClaimRepository> _groupClaimRepository;
        private Mock<IOperationClaimRepository> _operationClaimRepository;

        private CreateGroupClaimCommandHandler _createGroupClaimCommandHandler;
        private DeleteGroupClaimCommandHandler _deleteGroupClaimCommandHandler;
        private UpdateGroupClaimCommandHandler _updateGroupClaimCommandHandler;

        private GetGroupClaimQueryHandler _getGroupClaimQueryHandler;
        private GetGroupClaimsLookupByGroupIdQueryHandler _getGroupClaimsLookupByGroupIdQueryHandler;
        private GetGroupClaimsQueryHandler _getGroupClaimsQueryHandler;
        [SetUp]
        public void Setup()
        {
            _groupClaimRepository = new Mock<IGroupClaimRepository>();
            _operationClaimRepository = new Mock<IOperationClaimRepository>();

            _createGroupClaimCommandHandler = new CreateGroupClaimCommandHandler(_operationClaimRepository.Object);
            _deleteGroupClaimCommandHandler = new DeleteGroupClaimCommandHandler(_groupClaimRepository.Object);
            _updateGroupClaimCommandHandler = new UpdateGroupClaimCommandHandler(_groupClaimRepository.Object);

            _getGroupClaimQueryHandler = new GetGroupClaimQueryHandler(_groupClaimRepository.Object);
            _getGroupClaimsLookupByGroupIdQueryHandler =
                new GetGroupClaimsLookupByGroupIdQueryHandler(_groupClaimRepository.Object);
            _getGroupClaimsQueryHandler = new GetGroupClaimsQueryHandler(_groupClaimRepository.Object);

        }

        [Test]
        public async Task Handler_CreateGroupClaim_OperationClaimExists()
        {

            var command = new CreateGroupClaimCommand
            {
                ClaimName = "test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<OperationClaim>(new OperationClaim()));

            _operationClaimRepository.Setup(x => x.Add(It.IsAny<OperationClaim>()));

            var result = await _createGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.OperationClaimExists);
        }

        [Test]
        public async Task Handler_CreateGroupClaim_Added()
        {

            var command = new CreateGroupClaimCommand
            {
                ClaimName = "test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .Returns(Task.FromResult<OperationClaim>(null));

            _operationClaimRepository.Setup(x => x.Add(It.IsAny<OperationClaim>()));

            var result = await _createGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }


        [Test]
        public async Task Handler_UpdateGroupClaim_Updated()
        {

            var command = new UpdateGroupClaimCommand()
            {
                GroupId = 1,
                Id = 2,
                ClaimIds = new int[]
                {
                    1, 2, 3, 23, 45, 55
                }
            };

            _groupClaimRepository.Setup(x => x.BulkInsert(
                It.IsAny<int>(), It.IsAny<IEnumerable<GroupClaim>>()));

            var result = await _updateGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }


        [Test]
        public async Task Handler_DeleteGroupClaim_GroupClaimNotFound()
        {

            var command = new DeleteGroupClaimCommand()
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
        public async Task Handler_DeleteGroupClaim_Deleted()
        {
            var command = new DeleteGroupClaimCommand()
            {
                Id = 1,

            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim()
                {
                    OperationClaim = new OperationClaim(),
                    ClaimId = 1,
                    Group = new Group(),
                    GroupId = 1
                }));

            _groupClaimRepository.Setup(x => x.Delete(It.IsAny<GroupClaim>()));

            var result = await _deleteGroupClaimCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task Handler_GetGroupClaim_Success()
        {
            var command = new GetGroupClaimQuery()
            {
                Id = 1,

            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim()
                {
                    OperationClaim = new OperationClaim(),
                    ClaimId = 1,
                    Group = new Group(),
                    GroupId = 1
                }));

            var result = await _getGroupClaimQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.ClaimId.Should().Be(1);
        }

        [Test]
        public async Task Handler_GetGroupClaimsLookupByGroupIdClaim_Success()
        {
            var command = new GetGroupClaimsLookupByGroupIdQuery()
            {
                GroupId = 1,

            };

            _groupClaimRepository.Setup(x => x.GetGroupClaimsSelectedList(
                    It.IsAny<int>()))
                .Returns(Task.FromResult<IEnumerable<SelectionItem>>(
                    new List<SelectionItem>()
                    {
                        new SelectionItem()
                        {
                            Id = 1,
                            IsDisabled = false,
                            Label = "Test",
                            ParentId = "asdasd"
                        },
                        new SelectionItem()
                        {
                            Id = 1,
                            IsDisabled = false,
                            Label = "Test2",
                            ParentId = "gdfgdfg"
                        }

                    }));

            var result = await _getGroupClaimsLookupByGroupIdQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.ToList().Count.Should().BeGreaterThan(1);
        }



        [Test]
        public async Task Handler_GetGroupClaims_Success()
        {
            var command = new GetGroupClaimsQuery();

            _groupClaimRepository.Setup(x => x.GetListAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<GroupClaim>>(
                    new List<GroupClaim>()
                    {
                        new GroupClaim()
                        {
                          OperationClaim = new OperationClaim(),
                          ClaimId = 1,
                          Group = new Group(),
                          GroupId = 1
                        },
                        new GroupClaim()
                        {
                            OperationClaim = new OperationClaim(),
                            ClaimId = 2,
                            Group = new Group(),
                            GroupId = 1
                        }

                    }));

            var result = await _getGroupClaimsQueryHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.ToList().Count.Should().BeGreaterThan(1);
        }
    }
}