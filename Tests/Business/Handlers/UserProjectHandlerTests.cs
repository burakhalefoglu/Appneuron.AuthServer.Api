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
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static Business.Handlers.UserProjects.Commands.CreateUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.DeleteUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.UpdateUserProjectCommand;
using static Business.Handlers.UserProjects.Queries.GetUserProjectQuery;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsQuery;
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
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getUserProjectQueryHandler =
                new GetUserProjectQueryHandler(_userProjectRepository.Object);
            _getUserProjectsQueryHandler =
                new GetUserProjectsQueryHandler(_userProjectRepository.Object);

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
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetUserProjectQueryHandler _getUserProjectQueryHandler;
        private GetUserProjectsQueryHandler _getUserProjectsQueryHandler;

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
                    ProjectId = 1,
                    UserId = 1
                });

            //Act
            var x = await _getUserProjectQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.UserId.Should().Be(1);
        }

        [Test]
        public async Task UserProject_GetInternalQuery_Success()
        {
            //Arrange
            var query = new GetUserProjectInternalQuery();

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject
                {
                    ProjectId = 1,
                    UserId = 1
                });

            //Act
            var x = await _getUserProjectInternalQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.UserId.Should().Be(1);
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
                        ProjectId = 1,
                        UserId = 1
                    },

                    new()
                    {
                        ProjectId = 2,
                        UserId = 1
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
                        ProjectId = 1,
                        UserId = 1
                    },

                    new()
                    {
                        ProjectId = 2,
                        UserId = 1
                    }
                }.AsQueryable());

            //Act
            var x = await _getUserProjectsInternalQueryHandler.Handle(query, new CancellationToken());

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
                ProjectId = 2,
                UserId = 1
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
                ProjectId = 1,
                UserId = 1
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
                ProjectId = 1,
                UserId = 1
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
                ProjectId = 1,
                UserId = 1
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
                Id = 1,
                ProjectId = 1,
                UserId = 1
            };


            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject
                {
                    ProjectId = 1,
                    UserId = 1
                });

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>()));

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
                Id = 1,
                ProjectId = 1,
                UserId = 1
            };


            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync((UserProject) null);

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>()));

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
                Id = 1
            };

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject
                {
                    ProjectId = 1,
                    UserId = 1
                });

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>()));

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
                Id = 1
            };

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync((UserProject) null);

            _userProjectRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<UserProject>()));

            var x = await _deleteUserProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.UserProjectNotFound);
        }
    }
}