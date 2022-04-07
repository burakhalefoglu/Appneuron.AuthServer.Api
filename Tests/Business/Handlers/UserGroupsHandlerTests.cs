using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Business.Constants;
using Business.Handlers.UserGroups.Commands;
using Business.Handlers.UserGroups.Queries;
using Business.Internals.Handlers.UserGroups.Commands;
using Business.Internals.Handlers.UserGroups.Queries;
using Business.Internals.Handlers.Users;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserGroups.Commands.CreateUserGroupCommand;
using static Business.Handlers.UserGroups.Commands.DeleteUserGroupCommand;
using static Business.Handlers.UserGroups.Commands.UpdateUserGroupCommand;
using static Business.Handlers.UserGroups.Queries.GetUserGroupsQuery;
using static Business.Handlers.UserGroups.Queries.GetUserGroupQuery;
using static Business.Internals.Handlers.UserGroups.Commands.CreateUserGroupInternalCommand;
using static Business.Internals.Handlers.UserGroups.Queries.GetUserGroupInternalQuery;


namespace Tests.Business.Handlers;

[TestFixture]
public class UserGroupsHandlerTests
{
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _userGroupRepository = new Mock<IUserGroupRepository>();
        _createUserGroupCommandHandler = new CreateUserGroupCommandHandler(_userGroupRepository.Object);
        _updateUserGroupCommandHandler =
            new UpdateUserGroupCommandHandler(_userGroupRepository.Object, _mediator.Object);
        _deleteUserGroupCommandHandler = new DeleteUserGroupCommandHandler(_userGroupRepository.Object);
        _getUserGroupsQueryHandler = new GetUserGroupsQueryHandler(_userGroupRepository.Object);
        _getUserGroupQueryHandler = new GetUserGroupQueryHandler(_userGroupRepository.Object);
        _createUserGroupInternalCommandHandler = new CreateUserGroupInternalCommandHandler(_userGroupRepository.Object);
        _getUserGroupInternalQueryHandler = new GetUserGroupInternalQueryHandler(_userGroupRepository.Object);
    }

    private Mock<IMediator> _mediator;
    private Mock<IUserGroupRepository> _userGroupRepository;
    private GetUserGroupsQueryHandler _getUserGroupsQueryHandler;
    private CreateUserGroupCommandHandler _createUserGroupCommandHandler;
    private UpdateUserGroupCommandHandler _updateUserGroupCommandHandler;
    private DeleteUserGroupCommandHandler _deleteUserGroupCommandHandler;
    private GetUserGroupQueryHandler _getUserGroupQueryHandler;
    private CreateUserGroupInternalCommandHandler _createUserGroupInternalCommandHandler;
    private GetUserGroupInternalQueryHandler _getUserGroupInternalQueryHandler;


    [Test]
    public void Handler_GetUserGroups_Success()
    {
        var userGroup = new UserGroup {GroupId = 1, UserId = 1, Status = true};
        _userGroupRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<UserGroup, bool>>>()))
            .ReturnsAsync(new List<UserGroup> {userGroup}.AsQueryable());

        var result = _getUserGroupsQueryHandler.Handle(new GetUserGroupsQuery(), new CancellationToken()).Result;
        result.Data.Should().HaveCount(1);
        result.Success.Should().BeTrue();
    }

    [Test]
    public void Handler_GetUserGroup_Success()
    {
        var query = new GetUserGroupQuery
        {
            UserId = 1
        };

        _userGroupRepository.Setup(x =>
            x.GetAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(new UserGroup
        {
            Status = true
        });

        var result = _getUserGroupQueryHandler.Handle(query, new CancellationToken()).Result;
        result.Success.Should().BeTrue();
    }


    [Test]
    public void Handler_GetUserGroupInternal_Success()
    {
        var query = new GetUserGroupInternalQuery
        {
            UserId = 1
        };

        _userGroupRepository.Setup(x =>
            x.GetAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(new UserGroup
        {
            Status = true
        });

        var result = _getUserGroupInternalQueryHandler.Handle(query, new CancellationToken()).Result;
        result.Success.Should().BeTrue();
    }


    [Test]
    public void Handler_CreateUserGroup_Success()
    {
        var createUserCommand = new CreateUserGroupCommand
        {
            UserId = 1,
            GroupId = 1
        };
        _userGroupRepository.Setup(x =>
            x.AnyAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(false);

        _userGroupRepository.Setup(x => x.Add(It.IsAny<UserGroup>()));

        var result = _createUserGroupCommandHandler.Handle(createUserCommand, new CancellationToken()).Result;
        result.Success.Should().BeTrue();
    }

    [Test]
    public void Handler_CreateUserGroupInternalCommand_Success()
    {
        var createUserCommand = new CreateUserGroupInternalCommand
        {
            UserId = 1,
            GroupId = 1
        };
        _userGroupRepository.Setup(x =>
            x.AnyAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(false);

        _userGroupRepository.Setup(x => x.Add(It.IsAny<UserGroup>()));

        var result = _createUserGroupInternalCommandHandler.Handle(createUserCommand, new CancellationToken()).Result;
        result.Success.Should().BeTrue();
    }

    [Test]
    public void Handler_UpdateUserGroup_Success()
    {
        var updateUserCommand = new UpdateUserGroupCommand
        {
            GroupId = new long[] {1},
            UserId = 1
        };
        _mediator.Setup(x => x.Send(It.IsAny<GetUserInternalQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SuccessDataResult<User>(new User()));

        _userGroupRepository.Setup(x => x
            .Add(It.IsAny<UserGroup>()));

        var result = _updateUserGroupCommandHandler.Handle(updateUserCommand, new CancellationToken()).Result;

        result.Success.Should().BeTrue();
    }

    [Test]
    public void Handler_UpdateUserGroup_UserNotFound()
    {
        var updateUserCommand = new UpdateUserGroupCommand
        {
            GroupId = new long[] {1, 2, 3},
            UserId = 1
        };
        _mediator.Setup(x => x.Send(It.IsAny<GetUserInternalQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SuccessDataResult<User>((User) null));

        _userGroupRepository.Setup(x => x
            .AddAsync(It.IsAny<UserGroup>()));

        var result = _updateUserGroupCommandHandler.Handle(updateUserCommand, new CancellationToken()).Result;

        result.Success.Should().BeFalse();
        result.Message.Should().Be(Messages.UserNotFound);
    }

    [Test]
    public void Handler_DeleteUser_Success()
    {
        var deleteUserCommand = new DeleteUserGroupCommand();
        _userGroupRepository.Setup(x =>
            x.GetAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(new UserGroup());

        _userGroupRepository.Setup(x =>
            x.UpdateAsync(It.IsAny<UserGroup>()));

        var result = _deleteUserGroupCommandHandler.Handle(deleteUserCommand, new CancellationToken()).Result;

        result.Success.Should().BeTrue();
    }
}