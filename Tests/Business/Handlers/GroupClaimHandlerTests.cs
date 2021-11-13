﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.GroupClaims.Commands;
using Business.Handlers.GroupClaims.Queries;
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
        [SetUp]
        public void Setup()
        {
            _groupClaimRepository = new Mock<IGroupClaimRepository>();
            _operationClaimRepository = new Mock<IOperationClaimRepository>();

            _createGroupClaimCommandHandler = new CreateGroupClaimCommandHandler(_groupClaimRepository.Object, _operationClaimRepository.Object);
            _deleteGroupClaimCommandHandler = new DeleteGroupClaimCommandHandler(_groupClaimRepository.Object);
            _updateGroupClaimCommandHandler = new UpdateGroupClaimCommandHandler(_groupClaimRepository.Object, _operationClaimRepository.Object);

            _getGroupClaimQueryHandler = new GetGroupClaimQueryHandler(_groupClaimRepository.Object);
            _getGroupClaimsLookupByGroupIdQueryHandler =
                new GetGroupClaimsLookupByGroupIdQueryHandler(_groupClaimRepository.Object);
            _getGroupClaimsQueryHandler = new GetGroupClaimsQueryHandler(_groupClaimRepository.Object);
        }

        private Mock<IGroupClaimRepository> _groupClaimRepository;
        private Mock<IOperationClaimRepository> _operationClaimRepository;

        private CreateGroupClaimCommandHandler _createGroupClaimCommandHandler;
        private DeleteGroupClaimCommandHandler _deleteGroupClaimCommandHandler;
        private UpdateGroupClaimCommandHandler _updateGroupClaimCommandHandler;

        private GetGroupClaimQueryHandler _getGroupClaimQueryHandler;
        private GetGroupClaimsLookupByGroupIdQueryHandler _getGroupClaimsLookupByGroupIdQueryHandler;
        private GetGroupClaimsQueryHandler _getGroupClaimsQueryHandler;

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
                .Returns(Task.FromResult<OperationClaim>(new OperationClaim()));
 
            
            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult<GroupClaim>(new GroupClaim()));

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
                .Returns(Task.FromResult<OperationClaim>(new OperationClaim()));
 
            
            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult<GroupClaim>(null));

            _operationClaimRepository.Setup(x => x.Add(It.IsAny<OperationClaim>()));
            
            var result = await _createGroupClaimCommandHandler.Handle(command, new CancellationToken());

            _groupClaimRepository.Verify(x => x.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }


        [Test]
        public async Task GroupClaim_UpdateGroupClaim_Updated()
        {
            var command = new UpdateGroupClaimCommand
            {
                GroupId = 1,
                Id = 2,
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
                       
            _groupClaimRepository.Verify(x => x.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task GroupClaim_UpdateGroupClaim_OperationClaimNotFound()
        {
            var command = new UpdateGroupClaimCommand
            {
                GroupId = 1,
                Id = 2,
               ClaimId = 1,
               ClaimName = "Test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .ReturnsAsync((OperationClaim)null);

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
                Id = 2,
               ClaimId = 1,
               ClaimName = "Test"
            };

            _operationClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<OperationClaim, bool>>>()))
                .ReturnsAsync(new OperationClaim());

            _groupClaimRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .ReturnsAsync((GroupClaim)null);

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
                Id = 1
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
                Id = 1
            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim
                {
                    OperationClaim = new OperationClaim(),
                    ClaimId = 1,
                    Group = new Group(),
                    GroupId = 1
                }));

            _groupClaimRepository.Setup(x => x.Delete(It.IsAny<GroupClaim>()));

            var result = await _deleteGroupClaimCommandHandler.Handle(command, new CancellationToken());
  
            _groupClaimRepository.Verify(x => x.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task GroupClaim_GetGroupClaim_Success()
        {
            var command = new GetGroupClaimQuery
            {
                Id = 1
            };

            _groupClaimRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult(new GroupClaim
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
        public async Task GroupClaim_GetGroupClaimsLookupByGroupIdClaim_Success()
        {
            var command = new GetGroupClaimsLookupByGroupIdQuery
            {
                GroupId = 1
            };

            _groupClaimRepository.Setup(x => x.GetGroupClaimsSelectedList(
                    It.IsAny<int>()))
                .Returns(Task.FromResult<IEnumerable<SelectionItem>>(
                    new List<SelectionItem>
                    {
                        new()
                        {
                            Id = 1,
                            IsDisabled = false,
                            Label = "Test",
                            ParentId = "asdasd"
                        },
                        new()
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
        public async Task GroupClaim_GetGroupClaims_Success()
        {
            var command = new GetGroupClaimsQuery();

            _groupClaimRepository.Setup(x => x.GetListAsync(
                    It.IsAny<Expression<Func<GroupClaim, bool>>>()))
                .Returns(Task.FromResult<IEnumerable<GroupClaim>>(
                    new List<GroupClaim>
                    {
                        new()
                        {
                            OperationClaim = new OperationClaim(),
                            ClaimId = 1,
                            Group = new Group(),
                            GroupId = 1
                        },
                        new()
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