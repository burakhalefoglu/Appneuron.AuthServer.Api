using Business.Constants;
using Business.Handlers.UserProjects.Commands;
using Business.Handlers.UserProjects.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using Tests.Helpers.Token;
using static Business.Handlers.UserProjects.Commands.CreateUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.DeleteUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.UpdateUserProjectCommand;
using static Business.Handlers.UserProjects.Queries.GetUserProjectQuery;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsQuery;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsByUserIdQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UserProjectHandlerTests
    {
        private Mock<IUserProjectRepository> _userProjectRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetUserProjectQueryHandler _getUserProjectQueryHandler;
        private GetUserProjectsQueryHandler _getUserProjectsQueryHandler;
        private GetUserProjectsByUserIdQueryHandler _getUserProjectsByUserIdQueryHandler;

        private CreateUserProjectCommandHandler _createUserProjectCommandHandler;
        private UpdateUserProjectCommandHandler _updateUserProjectCommandHandler;
        private DeleteUserProjectCommandHandler _deleteUserProjectCommandHandler;

        [SetUp]
        public void Setup()
        {
            _userProjectRepository = new Mock<IUserProjectRepository>();
            _mediator = new Mock<IMediator>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getUserProjectQueryHandler = new GetUserProjectQueryHandler(_userProjectRepository.Object, _mediator.Object);
            _getUserProjectsQueryHandler = new GetUserProjectsQueryHandler(_userProjectRepository.Object, _mediator.Object);
            _getUserProjectsByUserIdQueryHandler = new GetUserProjectsByUserIdQueryHandler(
                _userProjectRepository.Object, _mediator.Object, _httpContextAccessor.Object);

            _createUserProjectCommandHandler = new CreateUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            _updateUserProjectCommandHandler = new UpdateUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            _deleteUserProjectCommandHandler = new DeleteUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);

        }

        [Test]
        public async Task UserProject_GetQuery_Success()
        {
            //Arrange
            var query = new GetUserProjectQuery();

            _userProjectRepository.Setup(x => x.
                GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .ReturnsAsync(new UserProject()
                {
                    Id = 1,
                    ProjectKey = "sadsfdsfsad",
                    UserId = 12
                });

            //Act
            var x = await _getUserProjectQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
        }

        [Test]
        public async Task UserProject_GetQueries_Success()
        {
            //Arrange
            var query = new GetUserProjectsQuery();

            _userProjectRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new List<UserProject> {

                            new UserProject()
                            {
                                Id = 1,
                                ProjectKey = "sdfsdfsdf",
                                UserId = 1
                            },

                            new UserProject()
                            {
                                Id = 12,
                                ProjectKey = "dasdsa",
                                UserId = 2
                            },

                        });

            //Act
            var x = await _getUserProjectsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<UserProject>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task UserProject_GetUserProjectsByUserId_Success()
        {
            //Arrange
            var query = new GetUserProjectsByUserIdQuery();

            _userProjectRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new List<UserProject> {

                            new UserProject()
                            {
                                Id = 1,
                                ProjectKey = "sdfsdfsdf",
                                UserId = 1
                            },

                            new UserProject()
                            {
                                Id = 12,
                                ProjectKey = "dasdsa",
                                UserId = 2
                            },

                        });


            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());


            //Act
            var x = await _getUserProjectsByUserIdQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<UserProject>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task UserProject_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateUserProjectCommand
            {
                ProjectKey = "fsdfsdffsd",
                UserId = 1
            };

            _userProjectRepository.Setup(x => x
                    .GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                .Returns(Task.FromResult<UserProject>(null));

            _userProjectRepository.Setup(x =>
                x.Add(It.IsAny<UserProject>())).Returns(new UserProject
                {
                    Id = 1,
                    ProjectKey = "fsdfsdffsd",
                    UserId = 1
                });

            var x = await _createUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task UserProject_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateUserProjectCommand
            {
                ProjectKey = "fsdfsdffsd",
                UserId = 1
            };

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                                           .Returns(Task.FromResult<UserProject>(new UserProject()));

            _userProjectRepository.Setup(x => x.Add(It.IsAny<UserProject>())).Returns(new UserProject());

            var x = await _createUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

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
                ProjectKey = "fdsfsdf",
                UserId = 1
            };


            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new UserProject()
                        {
                            Id = 1,
                            ProjectKey = "kjhkj",
                            UserId = 1
                        });

            _userProjectRepository.Setup(x => x.Update(It.IsAny<UserProject>())).Returns(new UserProject());

            var x = await _updateUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _userProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
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
                        .ReturnsAsync(new UserProject()
                        {
                            Id = 1,
                            ProjectKey = "fsdfsdf",
                            UserId = 1
                        });

            _userProjectRepository.Setup(x => x.Delete(It.IsAny<UserProject>()));

            var x = await _deleteUserProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _userProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}