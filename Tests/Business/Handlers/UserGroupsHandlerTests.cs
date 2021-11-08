using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Business.Handlers.UserGroups.Commands;
using Business.Handlers.UserGroups.Queries;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserGroups.Commands.CreateUserGroupCommand;
using static Business.Handlers.UserGroups.Commands.CreateUserGroupClaimsCommand;
using static Business.Handlers.UserGroups.Commands.DeleteUserGroupCommand;
using static Business.Handlers.UserGroups.Commands.UpdateUserGroupCommand;
using static Business.Handlers.UserGroups.Commands.UpdateUserGroupByGroupIdCommand;
using static Business.Handlers.UserGroups.Queries.GetUserGroupsQuery;
using static Business.Handlers.UserGroups.Queries.GetUserGroupLookupByUserIdQuery;
using static Business.Handlers.UserGroups.Queries.GetUserGroupQuery;
using static Business.Handlers.UserGroups.Queries.GetUsersInGroupLookupByGroupIdQuery;
using static Business.Handlers.UserGroups.Queries.GetUserGroupLookupQuery;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UserGroupsHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _userGroupRepository = new Mock<IUserGroupRepository>();
            _mediator = new Mock<IMediator>();

            _createUserGroupCommandHandler = new CreateUserGroupCommandHandler(_userGroupRepository.Object);
            _updateUserGroupCommandHandler = new UpdateUserGroupCommandHandler(_userGroupRepository.Object);
            _deleteUserGroupCommandHandler = new DeleteUserGroupCommandHandler(_userGroupRepository.Object);
            _createUserGroupClaimsCommandHandler = new CreateUserGroupClaimsCommandHandler(_userGroupRepository.Object);
            _getUserGroupsQueryHandler = new GetUserGroupsQueryHandler(_userGroupRepository.Object);
            _updateUserGroupByGroupIdCommandHandler =
                new UpdateUserGroupByGroupIdCommandHandler(_userGroupRepository.Object);
            _getUserGroupLookupByUserIdQueryHandler = new GetUserGroupLookupByUserIdQueryHandler(
                _userGroupRepository.Object, _mediator.Object);
            _getUserGroupQueryHandler = new GetUserGroupQueryHandler(_userGroupRepository.Object);
            _getUsersInGroupLookupByGroupIdQueryHandler =
                new GetUsersInGroupLookupByGroupIdQueryHandler(_userGroupRepository.Object);
            _getUserGroupLookupQueryHandler = new GetUserGroupLookupQueryHandler(_userGroupRepository.Object);
        }

        private Mock<IUserGroupRepository> _userGroupRepository;
        private Mock<IMediator> _mediator;

        private GetUserGroupsQueryHandler _getUserGroupsQueryHandler;
        private CreateUserGroupCommandHandler _createUserGroupCommandHandler;
        private UpdateUserGroupCommandHandler _updateUserGroupCommandHandler;
        private DeleteUserGroupCommandHandler _deleteUserGroupCommandHandler;
        private CreateUserGroupClaimsCommandHandler _createUserGroupClaimsCommandHandler;
        private UpdateUserGroupByGroupIdCommandHandler _updateUserGroupByGroupIdCommandHandler;
        private GetUserGroupLookupByUserIdQueryHandler _getUserGroupLookupByUserIdQueryHandler;
        private GetUserGroupQueryHandler _getUserGroupQueryHandler;
        private GetUsersInGroupLookupByGroupIdQueryHandler _getUsersInGroupLookupByGroupIdQueryHandler;
        private GetUserGroupLookupQueryHandler _getUserGroupLookupQueryHandler;

        [Test]
        public void Handler_GetUserGroups_Success()
        {
            var userGroup = new UserGroup { GroupId = 1, UserId = 1 };
            _userGroupRepository.Setup(x => x.GetListAsync(null))
                .ReturnsAsync(new List<UserGroup> { userGroup }.AsQueryable());

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
                x.GetAsync(It.IsAny<Expression<Func<UserGroup, bool>>>())).ReturnsAsync(new UserGroup());

            var result = _getUserGroupQueryHandler.Handle(query, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }


        [Test]
        public void Handler_GetUsersInGroupLookupByGroupId_Success()
        {
            var query = new GetUsersInGroupLookupByGroupIdQuery
            {
                GroupId = 1
            };

            _userGroupRepository.Setup(x =>
                    x.GetUsersInGroupSelectedListByGroupId(It.IsAny<int>()))
                .ReturnsAsync(new List<SelectionItem>().AsQueryable());

            var result = _getUsersInGroupLookupByGroupIdQueryHandler.Handle(query, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }


        [Test]
        public void Handler_GetUserGroupLookupQuery_Success()
        {
            var query = new GetUserGroupLookupQuery
            {
                UserId = 1
            };

            _userGroupRepository.Setup(x =>
                x.GetUserGroupSelectedList(It.IsAny<int>())).ReturnsAsync(new List<SelectionItem>().AsQueryable());

            var result = _getUserGroupLookupQueryHandler.Handle(query, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }

        [Test]
        public void Handler_GetUserGroupLookupByUserIdQuery_Success()
        {
            var query = new GetUserGroupLookupByUserIdQuery
            {
                UserId = 1
            };

            _userGroupRepository.Setup(x =>
                x.GetUserGroupSelectedList(It.IsAny<int>())).ReturnsAsync(new List<SelectionItem>().AsQueryable());

            var result = _getUserGroupLookupByUserIdQueryHandler.Handle(query, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }

        [Test]
        public void Handler_CreateUserGroup_Success()
        {
            var createUserCommand = new CreateUserGroupCommand();
            createUserCommand.UserId = 1;
            createUserCommand.GroupId = 1;

            _userGroupRepository.Setup(x => x.Add(It.IsAny<UserGroup>()));

            var result = _createUserGroupCommandHandler.Handle(createUserCommand, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }

        [Test]
        public void Handler_CreateUserGroupClaims_Success()
        {
            var createUserCommand = new CreateUserGroupClaimsCommand
            {
                UserId = 1,
                UserGroups = new List<UserGroup>()
            };


            _userGroupRepository.Setup(x => x.Add(It.IsAny<UserGroup>()));

            var result = _createUserGroupClaimsCommandHandler.Handle(createUserCommand, new CancellationToken()).Result;
            result.Success.Should().BeTrue();
        }

        [Test]
        public void Handler_UpdateUserGroup_Success()
        {
            var updateUserCommand = new UpdateUserGroupCommand();
            updateUserCommand.GroupId = new[] { 1 };
            updateUserCommand.UserId = 1;

            _userGroupRepository.Setup(x => x
                .BulkInsert(It.IsAny<int>(), It.IsAny<IEnumerable<UserGroup>>()));


            var result = _updateUserGroupCommandHandler.Handle(updateUserCommand, new CancellationToken()).Result;

            result.Success.Should().BeTrue();
        }

        [Test]
        public void Handler_UpdateUserGroupByGroupId_Success()
        {
            var updateUserCommand = new UpdateUserGroupByGroupIdCommand
            {
                GroupId = 1,
                Id = 1,
                UserIds = new[]
                {
                    1, 2, 5, 11
                }
            };

            _userGroupRepository.Setup(x => x
                .BulkInsert(It.IsAny<int>(), It.IsAny<IEnumerable<UserGroup>>()));

            var result = _updateUserGroupByGroupIdCommandHandler.Handle(updateUserCommand, new CancellationToken())
                .Result;
            result.Success.Should().BeTrue();
        }


        [Test]
        public void Handler_DeleteUser_Success()
        {
            var deleteUserCommand = new DeleteUserGroupCommand();
            var result = _deleteUserGroupCommandHandler.Handle(deleteUserCommand, new CancellationToken()).Result;

            result.Success.Should().BeTrue();
        }
    }
}