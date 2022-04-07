using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Groups.Commands;
using Business.Handlers.Groups.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Groups.Commands.CreateGroupCommand;
using static Business.Handlers.Groups.Commands.DeleteGroupCommand;
using static Business.Handlers.Groups.Commands.UpdateGroupCommand;
using static Business.Handlers.Groups.Queries.GetGroupQuery;
using static Business.Handlers.Groups.Queries.GetGroupsQuery;

namespace Tests.Business.Handlers;

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
        _getGroupQueryHandler = new GetGroupQueryHandler(_groupRepository.Object);
        _getGroupsQueryHandler = new GetGroupsQueryHandler(_groupRepository.Object);
    }

    private Mock<IGroupRepository> _groupRepository;

    private CreateGroupCommandHandler _createGroupCommandHandler;
    private DeleteGroupCommandHandler _deleteGroupCommandHandler;
    private UpdateGroupCommandHandler _updateGroupCommandHandler;
    private GetGroupQueryHandler _getGroupQueryHandler;
    private GetGroupsQueryHandler _getGroupsQueryHandler;

    [Test]
    public async Task Group_CreateGroup_Added()
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
    public async Task Group_CreateGroup_NameAlreadyExist()
    {
        var groupCommand = new CreateGroupCommand
        {
            GroupName = "ti"
        };

        _groupRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Group, bool>>>()))
            .ReturnsAsync(new Group());

        _groupRepository.Setup(x => x.Add(It.IsAny<Group>()));

        var result = await _createGroupCommandHandler.Handle(groupCommand, new CancellationToken());

        result.Success.Should().BeFalse();
        result.Message.Should().Be(Messages.NameAlreadyExist);
    }

    [Test]
    public async Task Group_DeleteGroup_Deleted()
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
    public async Task Group_DeleteGroup_GroupNotFound()
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
    public async Task Group_UpdateGroup_Updated()
    {
        var groupCommand = new UpdateGroupCommand
        {
            GroupName = "Test"
        };

        _groupRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Group, bool>>>()))
            .ReturnsAsync(new Group());

        _groupRepository.Setup(x =>
            x.Update(It.IsAny<Group>()));

        var result = await _updateGroupCommandHandler.Handle(groupCommand, new CancellationToken());
        result.Success.Should().BeTrue();
        result.Message.Should().Be(Messages.Updated);
    }

    [Test]
    public async Task Group_UpdateGroup_GroupNotFound()
    {
        var groupCommand = new UpdateGroupCommand
        {
            GroupName = "Test"
        };

        _groupRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Group, bool>>>()))
            .ReturnsAsync((Group) null);

        _groupRepository.Setup(x =>
            x.Update(It.IsAny<Group>()));

        var result = await _updateGroupCommandHandler.Handle(groupCommand, new CancellationToken());

        result.Success.Should().BeFalse();
        result.Message.Should().Be(Messages.GroupNotFound);
    }


    [Test]
    public async Task Group_GetGroup_Success()
    {
        var groupCommand = new GetGroupQuery();

        _groupRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<Group, bool>>>()))
            .ReturnsAsync(new Group
            {
                GroupName = "Test"
            });

        var result = await _getGroupQueryHandler.Handle(groupCommand, new CancellationToken());

        result.Success.Should().BeTrue();
        result.Data.GroupName.Should().Be("Test");
    }


    [Test]
    public async Task Group_GetGroups_Success()
    {
        var groupCommand = new GetGroupsQuery();

        _groupRepository.Setup(x =>
                x.GetListAsync(It.IsAny<Expression<Func<Group, bool>>>()))
            .ReturnsAsync(new List<Group>
            {
                new()
                {
                    GroupName = "Test"
                },
                new()
                {
                    GroupName = "Test1"
                }
            }.AsQueryable());

        var result = await _getGroupsQueryHandler.Handle(groupCommand, new CancellationToken());

        result.Success.Should().BeTrue();
        result.Data.Count().Should().BeGreaterThan(1);
    }
}