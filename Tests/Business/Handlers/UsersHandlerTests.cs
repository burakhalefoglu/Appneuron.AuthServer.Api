using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Constants;
using Business.Handlers.Users.Commands;
using Business.Handlers.Users.Queries;
using Core.Entities.Concrete;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Tests.Helpers;
using static Business.Handlers.Users.Commands.CreateUserCommand;
using static Business.Handlers.Users.Commands.DeleteUserCommand;
using static Business.Handlers.Users.Commands.UpdateUserCommand;
using static Business.Handlers.Users.Commands.UserChangePasswordCommand;
using static Business.Handlers.Users.Queries.GetUserLookupQuery;
using static Business.Handlers.Users.Queries.GetUserQuery;
using static Business.Handlers.Users.Queries.GetUsersQuery;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class UsersHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _mediator = new Mock<IMediator>();
            _mapper = new Mock<IMapper>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _createUserCommandHandler = new CreateUserCommandHandler(_userRepository.Object);
            _deleteUserCommandHandler =
                new DeleteUserCommandHandler(_userRepository.Object, _mapper.Object,
                    _mediator.Object, _httpContextAccessor.Object);
            _updateUserCommandHandler = new UpdateUserCommandHandler(_userRepository.Object,
                _mapper.Object, _httpContextAccessor.Object);
            _updateChangePasswordCommandHandler = new UserChangePasswordCommandHandler(_userRepository.Object,
                _mediator.Object, _mapper.Object, _httpContextAccessor.Object);
            _getUserLookupQueryHandler = new GetUserLookupQueryHandler(_userRepository.Object);
            _getUserQueryHandler = new GetUserQueryHandler(_userRepository.Object, _mapper.Object,
                _httpContextAccessor.Object);
            _getUsersQueryHandler = new GetUsersQueryHandler(_userRepository.Object, _mapper.Object);
        }

        private Mock<IUserRepository> _userRepository;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private CreateUserCommandHandler _createUserCommandHandler;
        private DeleteUserCommandHandler _deleteUserCommandHandler;
        private UpdateUserCommandHandler _updateUserCommandHandler;
        private UserChangePasswordCommandHandler _updateChangePasswordCommandHandler;
        private GetUserLookupQueryHandler _getUserLookupQueryHandler;
        private GetUserQueryHandler _getUserQueryHandler;
        private GetUsersQueryHandler _getUsersQueryHandler;

        [Test]
        public async Task User_CreateUser_Success()
        {
            var command = new CreateUserCommand
            {
                Email = "info@AdminClientBuilder.com",
                Password = "sdfsdfsdfsdf"
            };


            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult<User>(null));

            _userRepository.Setup(x => x.Add(It.IsAny<User>()));


            var result = await _createUserCommandHandler.Handle(command, new CancellationToken());
            
            _userRepository.Verify(c=> c.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task User_CreateUser_NameAlreadyExist()
        {
            var command = new CreateUserCommand
            {
                Email = "info@AdminClientBuilder.com",
                Password = "sdfsdfsdfsdf"
            };

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(new User()));

            _userRepository.Setup(x => x.Add(It.IsAny<User>()));

            var result = await _createUserCommandHandler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.NameAlreadyExist);
        }


        [Test]
        public async Task User_DeleteUser_Deleted()
        {
            var command = new DeleteUserCommand();


            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.Get(
                    It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new User());

            _userRepository.Setup(x => x.Delete(It.IsAny<User>()));

            var result = await _deleteUserCommandHandler.Handle(command, new CancellationToken());

            _userRepository.Verify(c=> c.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task User_DeleteUser_UserNotFound()
        {
            var command = new DeleteUserCommand();


            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.Get(
                    It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(await Task.FromResult<User>(null));

            _userRepository.Setup(x => x.Delete(It.IsAny<User>()));

            var result = await _deleteUserCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserNotFound);
        }

        [Test]
        public async Task User_UpdateUser_UserNotFound()
        {
            var command = new UpdateUserCommand
            {
                Email = "info@admin.com",
                FullName = "TestName"
            };

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult<User>(null));

            _userRepository.Setup(x => x.Update(It.IsAny<User>()));

            var result = await _updateUserCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserNotFound);
        }

        [Test]
        public async Task User_UpdateUser_Updated()
        {
            var command = new UpdateUserCommand
            {
                Email = "info@admin.com",
                FullName = "TestName"
            };

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(new User()));

            _userRepository.Setup(x => x.Update(It.IsAny<User>()));

            var result = await _updateUserCommandHandler.Handle(command, new CancellationToken());

            _userRepository.Verify(c=> c.SaveChangesAsync());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task User_UserChangePassword_UserNotFound()
        {
            var command = new UserChangePasswordCommand
            {
                Password = "123456",
                ValidPassword = "123456"
            };

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult<User>(null));

            _userRepository.Setup(x => x.Update(It.IsAny<User>()));

            var result = await _updateChangePasswordCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserNotFound);
        }

        [Test]
        public async Task User_UserChangePassword_PasswordError()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;


            var command = new UserChangePasswordCommand
            {
                Password = "1234567",
                ValidPassword = "1234567"
            };

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(user));

            _userRepository.Setup(x => x.Update(It.IsAny<User>()));

            var result = await _updateChangePasswordCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.PasswordError);
        }


        [Test]
        public async Task User_UserChangePassword_Updated()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;


            var command = new UserChangePasswordCommand
            {
                Password = "123456",
                ValidPassword = "123456"
            };

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(user));

            _userRepository.Setup(x => x.Update(It.IsAny<User>()));

            var result = await _updateChangePasswordCommandHandler.Handle(command, new CancellationToken());

            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task User_GetUserLookup_Success()
        {
            var query = new GetUserLookupQuery();

            _userRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User>
                {
                    new(),
                    new()
                });

            var result = await _getUserLookupQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Data.Count().Should().BeGreaterThan(1);
        }

        [Test]
        public async Task User_GetUser_Success()
        {
            var query = new GetUserQuery();

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(new User());

            var result = await _getUserQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
        }

        [Test]
        public async Task User_GetUsers_Success()
        {
            var query = new GetUsersQuery();

            _httpContextAccessor.SetupGet(x =>
                x.HttpContext).Returns(new DefaultHttpContext());

            _userRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User>
                {
                    new(),
                    new()
                });

            var result = await _getUsersQueryHandler.Handle(query, new CancellationToken());
            result.Success.Should().BeTrue();
        }
    }
}