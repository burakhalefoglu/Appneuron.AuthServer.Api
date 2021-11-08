using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Groups.Commands;
using Business.Handlers.Groups.Queries;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Groups.Commands.CreateGroupCommand;
using static Business.Handlers.Groups.Commands.DeleteGroupCommand;
using static Business.Handlers.Groups.Commands.UpdateGroupCommand;
using static Business.Handlers.Groups.Queries.SearchGroupsByNameQuery;
using static Business.Handlers.Groups.Queries.GetGroupLookupQuery;
using static Business.Handlers.Groups.Queries.GetGroupQuery;
using static Business.Handlers.Groups.Queries.GetGroupsQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class GroupHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _groupRepository = new Mock<IGroupRepository>();
            _createGroupCommandHandler = new CreateGroupCommandHandler(_groupRepository.Object);
            _deleteGroupCommandHandler = new DeleteGroupCommandHandler(_groupRepository.Object);
            _updateGroupCommandHandler = new UpdateGroupCommandHandler(_groupRepository.Object);
            _getGroupLookupQueryHandler = new GetGroupLookupQueryHandler(_groupRepository.Object);
            _getGroupQueryHandler = new GetGroupQueryHandler(_groupRepository.Object);
            _getGroupsQueryHandler = new GetGroupsQueryHandler(_groupRepository.Object);
            _searchGroupsByNameQueryHandler = new SearchGroupsByNameQueryHandler(_groupRepository.Object);
        }

        private Mock<IGroupRepository> _groupRepository;

        private CreateGroupCommandHandler _createGroupCommandHandler;
        private DeleteGroupCommandHandler _deleteGroupCommandHandler;
        private UpdateGroupCommandHandler _updateGroupCommandHandler;
        private GetGroupLookupQueryHandler _getGroupLookupQueryHandler;
        private GetGroupQueryHandler _getGroupQueryHandler;
        private GetGroupsQueryHandler _getGroupsQueryHandler;
        private SearchGroupsByNameQueryHandler _searchGroupsByNameQueryHandler;

        [Test]
        public async Task Handler_CreateGroup_Added()
        {
            var groupCommand = new CreateGroupCommand
            {
                GroupName = "ti"
            };

            _groupRepository.Setup(x => x.Add(It.IsAny<Group>()));

            var result = await _createGroupCommandHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Handler_DeleteGroup_Deleted()
        {
            var groupCommand = new DeleteGroupCommand
            {
                Id = 1
            };

            _groupRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .Returns(Task.FromResult(new Group()));

            var result = await _deleteGroupCommandHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task Handler_DeleteGroup_GroupNotFound()
        {
            var groupCommand = new DeleteGroupCommand
            {
                Id = 1
            };

            _groupRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .Returns(Task.FromResult<Group>(null));

            var result = await _deleteGroupCommandHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.GroupNotFound);
        }


        [Test]
        public async Task Handler_UpdateGroup_Updated()
        {
            var groupCommand = new UpdateGroupCommand
            {
                Id = 1,
                GroupName = "Test"
            };

            _groupRepository.Setup(x =>
                x.Update(It.IsAny<Group>()));

            var result = await _updateGroupCommandHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }


        [Test]
        public async Task Handler_GetGroupLookUpGroup_Success()
        {
            var groupCommand = new GetGroupLookupQuery();

            _groupRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .ReturnsAsync(new List<Group>
                {
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test",
                        Id = 1,
                        UserGroups = new List<UserGroup>()
                    },
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test1",
                        Id = 2,
                        UserGroups = new List<UserGroup>()
                    }
                });

            var result = await _getGroupLookupQueryHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Data.Count().Should().Be(2);
            Assert.AreEqual("1", result.Data.ToArray()[0].Id);
            Assert.AreEqual("Test", result.Data.ToArray()[0].Label);
        }


        [Test]
        public async Task Handler_GetGroup_Success()
        {
            var groupCommand = new GetGroupQuery();

            _groupRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .ReturnsAsync(new Group
                {
                    GroupClaims = new List<GroupClaim>(),
                    GroupName = "Test",
                    Id = 1,
                    UserGroups = new List<UserGroup>()
                });

            var result = await _getGroupQueryHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Data.GroupName.Should().Be("Test");
        }


        [Test]
        public async Task Handler_GetGroups_Success()
        {
            var groupCommand = new GetGroupsQuery();

            _groupRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .ReturnsAsync(new List<Group>
                {
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test",
                        Id = 1,
                        UserGroups = new List<UserGroup>()
                    },
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test1",
                        Id = 2,
                        UserGroups = new List<UserGroup>()
                    }
                });

            var result = await _getGroupsQueryHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }


        [Test]
        public async Task Handler_SearchGroupsByName_StringLengthMustBeGreaterThanThree()
        {
            var groupCommand = new SearchGroupsByNameQuery();
            groupCommand.GroupName = "Te";

            _groupRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .ReturnsAsync(new List<Group>
                {
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test",
                        Id = 1,
                        UserGroups = new List<UserGroup>()
                    },
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test",
                        Id = 2,
                        UserGroups = new List<UserGroup>()
                    }
                });

            var result = await _searchGroupsByNameQueryHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.StringLengthMustBeGreaterThanThree);
        }

        [Test]
        public async Task Handler_SearchGroupsByName_Success()
        {
            var groupCommand = new SearchGroupsByNameQuery();
            groupCommand.GroupName = "Test";

            _groupRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .ReturnsAsync(new List<Group>
                {
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test",
                        Id = 1,
                        UserGroups = new List<UserGroup>()
                    },
                    new()
                    {
                        GroupClaims = new List<GroupClaim>(),
                        GroupName = "Test",
                        Id = 2,
                        UserGroups = new List<UserGroup>()
                    }
                });

            var result = await _searchGroupsByNameQueryHandler.Handle(groupCommand, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }
    }
}