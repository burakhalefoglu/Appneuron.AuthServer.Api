using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Business.Handlers.UserGroups.Commands;
using Business.Handlers.UserGroups.Queries;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserGroups.Commands.CreateUserGroupCommand;
using static Business.Handlers.UserGroups.Commands.DeleteUserGroupCommand;
using static Business.Handlers.UserGroups.Commands.UpdateUserGroupCommand;
using static Business.Handlers.UserGroups.Queries.GetUserGroupsQuery;
using static Business.Handlers.UserGroups.Queries.GetUserGroupQuery;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UserGroupsHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _userGroupRepository = new Mock<IUserGroupRepository>();
            _createUserGroupCommandHandler = new CreateUserGroupCommandHandler(_userGroupRepository.Object);
            _updateUserGroupCommandHandler = new UpdateUserGroupCommandHandler(_userGroupRepository.Object);
            _deleteUserGroupCommandHandler = new DeleteUserGroupCommandHandler(_userGroupRepository.Object);
            _getUserGroupsQueryHandler = new GetUserGroupsQueryHandler(_userGroupRepository.Object);
            _getUserGroupQueryHandler = new GetUserGroupQueryHandler(_userGroupRepository.Object);
        }

        private Mock<IUserGroupRepository> _userGroupRepository;
        private GetUserGroupsQueryHandler _getUserGroupsQueryHandler;
        private CreateUserGroupCommandHandler _createUserGroupCommandHandler;
        private UpdateUserGroupCommandHandler _updateUserGroupCommandHandler;
        private DeleteUserGroupCommandHandler _deleteUserGroupCommandHandler;
        private GetUserGroupQueryHandler _getUserGroupQueryHandler;

        [Test]
        public void Handler_GetUserGroups_Success()
        {
            var userGroup = new UserGroup {GroupId = "test", UserId = "test"};
            _userGroupRepository.Setup(x => x.GetListAsync(null))
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
                UserId = "test_user_ıd"
            };

            _userGroupRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(new UserGroup());

            var result = _getUserGroupQueryHandler.Handle(query, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }


        [Test]
        public void Handler_CreateUserGroup_Success()
        {
            var createUserCommand = new CreateUserGroupCommand
            {
                UserId = "test_user_ıd",
                GroupId = "test_group_ıd"
            };

            _userGroupRepository.Setup(x => x.Add(It.IsAny<UserGroup>()));

            var result = _createUserGroupCommandHandler.Handle(createUserCommand, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }

        [Test]
        public void Handler_UpdateUserGroup_Success()
        {
            var updateUserCommand = new UpdateUserGroupCommand
            {
                GroupId = new[] {"test_group_ıd"},
                UserId = "test_user_ıd"
            };

            _userGroupRepository.Setup(x => x
                .AddManyAsync(It.IsAny<IEnumerable<UserGroup>>()));


            var result = _updateUserGroupCommandHandler.Handle(updateUserCommand, new CancellationToken()).Result;

            result.Success.Should().BeTrue();
        }

        [Test]
        public void Handler_DeleteUser_Success()
        {
            var deleteUserCommand = new DeleteUserGroupCommand();
            _userGroupRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(new UserGroup());

            _userGroupRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserGroup>(), It.IsAny<Expression<Func<UserGroup, bool>>>()));

            var result = _deleteUserGroupCommandHandler.Handle(deleteUserCommand, new CancellationToken()).Result;

            result.Success.Should().BeTrue();
        }
    }
}