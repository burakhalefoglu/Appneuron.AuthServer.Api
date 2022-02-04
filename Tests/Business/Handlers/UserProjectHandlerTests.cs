using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.UserProjects.Commands;
using Business.Handlers.UserProjects.Queries;
using Business.Internals.Handlers.UserProjects;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserProjects.Commands.CreateUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.DeleteUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.UpdateUserProjectCommand;
using static Business.Handlers.UserProjects.Queries.GetUserProjectQuery;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsQuery;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsByUserIdQuery;
using static Business.Internals.Handlers.UserProjects.CreateUserProjectInternalCommand;
using static Business.Internals.Handlers.UserProjects.GetUserProjectInternalQuery;
using static Business.Internals.Handlers.UserProjects.GetUserProjectsInternalQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UserProjectHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _userProjectRepository = new Mock<IUserProjectRepository>();
            _mediator = new Mock<IMediator>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getUserProjectQueryHandler =
                new GetUserProjectQueryHandler(_userProjectRepository.Object);
            _getUserProjectsQueryHandler =
                new GetUserProjectsQueryHandler(_userProjectRepository.Object);
            _getUserProjectsByUserIdQueryHandler = new GetUserProjectsByUserIdQueryHandler(
                _userProjectRepository.Object, _mediator.Object, _httpContextAccessor.Object);

            _createUserProjectCommandHandler =
                new CreateUserProjectCommandHandler(_userProjectRepository.Object);
            _updateUserProjectCommandHandler =
                new UpdateUserProjectCommandHandler(_userProjectRepository.Object);
            _deleteUserProjectCommandHandler =
                new DeleteUserProjectCommandHandler(_userProjectRepository.Object);

            _createUserProjectInternalCommandHandler = 
                new CreateUserProjectInternalCommandHandler(_userProjectRepository.Object);
            _getUserProjectInternalQueryHandler = 
                new GetUserProjectInternalQueryHandler(_userProjectRepository.Object);
            _getUserProjectsInternalQueryHandler = 
                new GetUserProjectsInternalQueryHandler(_userProjectRepository.Object);
        }

        private Mock<IUserProjectRepository> _userProjectRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetUserProjectQueryHandler _getUserProjectQueryHandler;
        private GetUserProjectsQueryHandler _getUserProjectsQueryHandler;
        private GetUserProjectsByUserIdQueryHandler _getUserProjectsByUserIdQueryHandler;

        private CreateUserProjectCommandHandler _createUserProjectCommandHandler;
        private UpdateUserProjectCommandHandler _updateUserProjectCommandHandler;
        private DeleteUserProjectCommandHandler _deleteUserProjectCommandHandler;

        private CreateUserProjectInternalCommandHandler _createUserProjectInternalCommandHandler;
        private GetUserProjectInternalQueryHandler _getUserProjectInternalQueryHandler;
        private GetUserProjectsInternalQueryHandler _getUserProjectsInternalQueryHandler;
        

        [Test]
        public async Task UserProject_GetQuery_Success()
        {
            //Arrange
            var query = new GetUserProjectQuery();

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject
                {
                    ProjectKey = "project_key",
                    UserId = "test_user_ıd"
                });

            //Act
            var x = await _getUserProjectQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.UserId.Should().Be("test_user_ıd");
        }

        [Test]
        public async Task UserProject_GetInternalQuery_Success()
        {
            //Arrange
            var query = new GetUserProjectInternalQuery();

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject
                {
                    ProjectKey = "project_key",
                    UserId = "test_user_ıd"
                });

            //Act
            var x = await _getUserProjectInternalQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.UserId.Should().Be("test_user_ıd");
        }

        [Test]
        public async Task UserProject_GetQueries_Success()
        {
            //Arrange
            var query = new GetUserProjectsQuery();

            _userProjectRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new List<UserProject>
                {
                    new()
                    {
                        ProjectKey = "project_key_1",
                        UserId = "test_user_ıd"
                    },

                    new()
                    {
                        ProjectKey = "project_key_2",
                        UserId = "test_user_ıd2"
                    }
                }.AsQueryable());

            //Act
            var x = await _getUserProjectsQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        
        [Test]
        public async Task UserProject_GetInternalQueries_Success()
        {
            //Arrange
            var query = new GetUserProjectsInternalQuery();

            _userProjectRepository.Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new List<UserProject>
                {
                    new()
                    {
                        ProjectKey = "project_key_1",
                        UserId = "test_user_ıd"
                    },

                    new()
                    {
                        ProjectKey = "project_key_2",
                        UserId = "test_user_ıd2"
                    }
                }.AsQueryable());

            //Act
            var x = await _getUserProjectsInternalQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task UserProject_GetUserProjectsByUserId_Success()
        {
            //Arrange
            var query = new GetUserProjectsByUserIdQuery();

            _userProjectRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new List<UserProject>
                {
                    new()
                    {
                        ProjectKey = "project_key_1",
                        UserId = "test_user_ıd"
                    },

                    new()
                    {
                        ProjectKey = "project_key_2",
                        UserId = "test_user_ıd2"
                    }
                }.AsQueryable());


            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());


            //Act
            var x = await _getUserProjectsByUserIdQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task UserProject_CreateInternalCommand_Success()
        {
            //Arrange
            var command = new CreateUserProjectInternalCommand
            {
                ProjectKey = "project_key_1",
                UserId = "test_user_ıd"
            };

            _userProjectRepository.Setup(x => x
                    .AnyAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(false);

            _userProjectRepository.Setup(x =>
                x.AddAsync(It.IsAny<UserProject>()));
            var x = await _createUserProjectInternalCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task UserProject_CreateInternalCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateUserProjectInternalCommand
            {
                ProjectKey = "project_key_1",
                UserId = "test_user_ıd"
            };

            _userProjectRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(true);

            _userProjectRepository.Setup(x => x.AddAsync(It.IsAny<UserProject>()));

            var x = await _createUserProjectInternalCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        
        [Test]
        public async Task UserProject_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateUserProjectCommand
            {
                ProjectKey = "project_key_1",
                UserId = "test_user_ıd"
            };

            _userProjectRepository.Setup(x => x
                    .AnyAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(false);

            _userProjectRepository.Setup(x =>
                x.AddAsync(It.IsAny<UserProject>()));
            var x = await _createUserProjectCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task UserProject_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateUserProjectCommand
            {
                ProjectKey = "project_key_1",
                UserId = "test_user_ıd"
            };

            _userProjectRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(true);

            _userProjectRepository.Setup(x => x.AddAsync(It.IsAny<UserProject>()));

            var x = await _createUserProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task UserProject_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateUserProjectCommand
            {
                Id = "test_ıd",
                ProjectKey = "project_key_1",
                UserId = "test_user_ıd"
            };


            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject
                {
                    ProjectKey = "project_key_1",
                    UserId = "test_user_ıd"
                });

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>(), It.IsAny<Expression<Func<UserProject, bool>>>()));

            var x = await _updateUserProjectCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task UserProject_UpdateCommand_UserProjectNotFound()
        {
            //Arrange
            var command = new UpdateUserProjectCommand
            {
                Id = "test_ıd",
                ProjectKey = "project_key_1",
                UserId = "test_user_ıd"
            };


            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync((UserProject) null);

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>(), It.IsAny<Expression<Func<UserProject, bool>>>()));

            var x = await _updateUserProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.UserProjectNotFound);
        }

        [Test]
        public async Task UserProject_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteUserProjectCommand
            {
                Id = "test_ıd"
            };

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject
                {
                    ProjectKey = "test",
                    UserId = "test_user_ıd"
                });

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>(), It.IsAny<Expression<Func<UserProject, bool>>>()));

            var x = await _deleteUserProjectCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task UserProject_DeleteCommand_UserProjectNotFound()
        {
            //Arrange
            var command = new DeleteUserProjectCommand
            {
                Id = "test_ıd"
            };

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync((UserProject) null);

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>(), It.IsAny<Expression<Func<UserProject, bool>>>()));

            var x = await _deleteUserProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.UserProjectNotFound);
        }
    }
}