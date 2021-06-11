
using Business.Handlers.UserProjects.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.UserProjects.Queries.GetUserProjectQuery;
using Entities.Concrete;
using static Business.Handlers.UserProjects.Queries.GetUserProjectsQuery;
using static Business.Handlers.UserProjects.Commands.CreateUserProjectCommand;
using Business.Handlers.UserProjects.Commands;
using Business.Constants;
using static Business.Handlers.UserProjects.Commands.UpdateUserProjectCommand;
using static Business.Handlers.UserProjects.Commands.DeleteUserProjectCommand;
using MediatR;
using System.Linq;
using FluentAssertions;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class UserProjectHandlerTests
    {
        Mock<IUserProjectRepository> _userProjectRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _userProjectRepository = new Mock<IUserProjectRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task UserProject_GetQuery_Success()
        {
            //Arrange
            var query = new GetUserProjectQuery();

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>())).ReturnsAsync(new UserProject()
//propertyler buraya yazılacak
//{																		
//UserProjectId = 1,
//UserProjectName = "Test"
//}
);

            var handler = new GetUserProjectQueryHandler(_userProjectRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.UserProjectId.Should().Be(1);

        }

        [Test]
        public async Task UserProject_GetQueries_Success()
        {
            //Arrange
            var query = new GetUserProjectsQuery();

            _userProjectRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new List<UserProject> { new UserProject() { /*TODO:propertyler buraya yazılacak UserProjectId = 1, UserProjectName = "test"*/ } });

            var handler = new GetUserProjectsQueryHandler(_userProjectRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<UserProject>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task UserProject_CreateCommand_Success()
        {
            UserProject rt = null;
            //Arrange
            var command = new CreateUserProjectCommand();
            //propertyler buraya yazılacak
            //command.UserProjectName = "deneme";

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(rt);

            _userProjectRepository.Setup(x => x.Add(It.IsAny<UserProject>())).Returns(new UserProject());

            var handler = new CreateUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _userProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task UserProject_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateUserProjectCommand();
            //propertyler buraya yazılacak 
            //command.UserProjectName = "test";

            _userProjectRepository.Setup(x => x.Query())
                                           .Returns(new List<UserProject> { new UserProject() { /*TODO:propertyler buraya yazılacak UserProjectId = 1, UserProjectName = "test"*/ } }.AsQueryable());

            _userProjectRepository.Setup(x => x.Add(It.IsAny<UserProject>())).Returns(new UserProject());

            var handler = new CreateUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task UserProject_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateUserProjectCommand();
            //command.UserProjectName = "test";

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new UserProject() { /*TODO:propertyler buraya yazılacak UserProjectId = 1, UserProjectName = "deneme"*/ });

            _userProjectRepository.Setup(x => x.Update(It.IsAny<UserProject>())).Returns(new UserProject());

            var handler = new UpdateUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _userProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task UserProject_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteUserProjectCommand();

            _userProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserProject, bool>>>()))
                        .ReturnsAsync(new UserProject() { /*TODO:propertyler buraya yazılacak UserProjectId = 1, UserProjectName = "deneme"*/});

            _userProjectRepository.Setup(x => x.Delete(It.IsAny<UserProject>()));

            var handler = new DeleteUserProjectCommandHandler(_userProjectRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _userProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

