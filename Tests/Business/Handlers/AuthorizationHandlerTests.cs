using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Authorizations.Commands;
using Business.Handlers.Authorizations.Queries;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.Groups.Queries;
using Business.Internals.Handlers.UserGroups.Commands;
using Business.Internals.Handlers.UserGroups.Queries;
using Business.Internals.Handlers.UserProjects;
using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Mail;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using Tests.Helpers;
using static Business.Handlers.Authorizations.Commands.ForgotPasswordCommand;
using static Business.Handlers.Authorizations.Commands.RegisterUserCommand;
using static Business.Handlers.Authorizations.Commands.ResetPasswordCommand;
using static Business.Handlers.Authorizations.Queries.LoginUserQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class AuthorizationHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _tokenHelper = new Mock<ITokenHelper>();
            _mediator = new Mock<IMediator>();
            _mailService = new Mock<IMailService>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _loginUserQueryHandler = new LoginUserQueryHandler(_userRepository.Object,
                _tokenHelper.Object,
                _mediator.Object);

            _registerUserCommandHandler = new RegisterUserCommandHandler(_userRepository.Object,
                _mediator.Object, _tokenHelper.Object);

            _forgotPasswordCommandHandler = new ForgotPasswordCommandHandler(_userRepository.Object,
                _mailService.Object);

            _resetPasswordCommandHandler = new ResetPasswordCommandHandler(_userRepository.Object,
                _mediator.Object,
                _httpContextAccessor.Object);
        }

        private Mock<IUserRepository> _userRepository;
        private Mock<ITokenHelper> _tokenHelper;
        private Mock<IMediator> _mediator;
        private Mock<IMailService> _mailService;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private LoginUserQueryHandler _loginUserQueryHandler;
        private LoginUserQuery _loginUserQuery;
        private RegisterUserCommandHandler _registerUserCommandHandler;
        private RegisterUserCommand _command;
        private ForgotPasswordCommandHandler _forgotPasswordCommandHandler;
        private ForgotPasswordCommand _forgotPasswordCommand;
        private ResetPasswordCommandHandler _resetPasswordCommandHandler;

        [Test]
        public async Task Authorization_Login_UserNotFount()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            _userRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult<User>(null));

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<SelectionItem>>(new List<SelectionItem>
                    {
                        new()
                        {
                            Id = 1,
                            Label = "test",
                            IsDisabled = false,
                            ParentId = "test"
                        }
                    }));
            _loginUserQuery = new LoginUserQuery
            {
                Email = user.Email,
                Password = "123456"
            };

            var result = await _loginUserQueryHandler.Handle(_loginUserQuery, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.DefaultError);
        }


        [Test]
        public async Task Authorization_Login_PasswordError()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _userRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(() => Task.FromResult(user));

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<SelectionItem>>(new List<SelectionItem>
                    {
                        new()
                        {
                            Id = 1,
                            Label = "test",
                            IsDisabled = false,
                            ParentId = "test"
                        }
                    }));

            _loginUserQuery = new LoginUserQuery
            {
                Email = user.Email,
                Password = "1234567"
            };

            var result = await _loginUserQueryHandler.Handle(_loginUserQuery, new CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.DefaultError);
        }


        [Test]
        public async Task Authorization_Login_Success()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _userRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(() => Task.FromResult(user));

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetUserGroupInternalQuery>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync( new SuccessDataResult<UserGroup>(new UserGroup
            {
                Id = new ObjectId("507f1f77bcf86cd799439011")
            }));

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<SelectionItem>>(new List<SelectionItem>
                    {
                        new()
                        {
                            Id = "507f1f77bcf86cd799439011",
                            Label = "test",
                            IsDisabled = false,
                            ParentId = "test"
                        }
                    }));

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetUserProjectsInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<UserProject>>(new List<UserProject>
                    {
                        new()
                        {
                            ProjectKey = "test_key"
                        }
                    }));


            _tokenHelper.Setup(x => x.CreateCustomerToken<AccessToken>(new UserClaimModel
            {
                OperationClaims = null
            }, new List<string>())).Returns(new AccessToken());


            _loginUserQuery = new LoginUserQuery
            {
                Email = user.Email,
                Password = "123456"
            };

            var result = await _loginUserQueryHandler.Handle(_loginUserQuery, new CancellationToken());

            result.Success.Should().BeTrue();
        }


        [Test]
        public async Task Authorization_Register_EmailAlreadyExist()
        {
            var registerUser = new User {Email = "test@test.com", Name = "test test"};
            _command = new RegisterUserCommand
            {
                Email = registerUser.Email,
                Password = "123456"
            };

            _userRepository.Setup(x =>
                    x.AnyAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);


            var result = await _registerUserCommandHandler.Handle(_command, new CancellationToken());

            result.Message.Should().Be(Messages.DefaultError);
        }


        [Test]
        public async Task Authorization_Register_SuccessfulLogin()
        {
            var registerUser = new User {Email = "test@test.com", Name = "test test"};
            _command = new RegisterUserCommand
            {
                Email = registerUser.Email,
                Password = "123456"
            };

            _userRepository.Setup(x =>
                    x.AnyAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false);

            _userRepository.Setup(x => x.AddAsync(It.IsAny<User>()));

            _userRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult<User>(new User()
                {
                    Id = new ObjectId("107f1f77acf86cd799439011"),
                    Email = registerUser.Email,
                }));
            
            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetGroupByNameInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<Group>(new Group()
                {
                    Id = new ObjectId("507f1f77bcf86cd799439011")
                }));
            
            _mediator.Setup(m =>
                    m.Send(It.IsAny<CreateUserGroupInternalCommand>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessResult());
            
            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<SelectionItem>>(new List<SelectionItem>
                    {
                        new()
                        {
                            Id = "507f1f77bcf86cd799439011",
                            Label = "test",
                            IsDisabled = false,
                            ParentId = "test"
                        }
                    }));

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetUserProjectsInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<UserProject>>(new List<UserProject>
                    {
                        new()
                        {
                            ProjectKey = "test_key"
                        }
                    }));

            _tokenHelper.Setup(x => x.CreateCustomerToken<AccessToken>(new UserClaimModel
            {
                UserId = "107f1f77acf86cd799439011",
                OperationClaims = null
            }, new List<string>())).Returns(new AccessToken());


            var result = await _registerUserCommandHandler.Handle(_command, new CancellationToken());

            result.Message.Should().Be(Messages.SuccessfulLogin);
        }


        [Test]
        public async Task Authorization_ForgotPassword_WrongEmail()
        {
            var user = DataHelper.GetUser("test");
            _userRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult<User>(null));

            _forgotPasswordCommand = new ForgotPasswordCommand
            {
                Email = user.Email
            };
            var result = await _forgotPasswordCommandHandler.Handle(_forgotPasswordCommand, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.SendPassword);
        }

        [Test]
        public async Task Authorization_ForgotPassword_Success()
        {
            var user = DataHelper.GetUser("test");
            _userRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult(user));

            _forgotPasswordCommand = new ForgotPasswordCommand
            {
                Email = user.Email
            };

            _mailService.Setup(x => x.Send(It.IsAny<EmailMessage>()));

            _userRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<Expression<Func<User, bool>>>()));

            var result = await _forgotPasswordCommandHandler.Handle(_forgotPasswordCommand, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.SendPassword);
        }


        [Test]
        public async Task Authorization_ResetPassword_InvalidCode()
        {
            var resetPasswordCommand = new ResetPasswordCommand
            {
                Password = "test_pass"
            };

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult<User>(null));

            _httpContextAccessor.Setup(x =>
                    x.HttpContext.Request.Query)
                .Returns(() => new QueryCollection());

            var result = await _resetPasswordCommandHandler.Handle(resetPasswordCommand, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.InvalidCode);
        }

        [Test]
        public async Task Authorization_ResetPassword_InvalidCodeWhenResPasExpiresLowerThanDatetimeNow()
        {
            var resetPasswordCommand = new ResetPasswordCommand
            {
                Password = "test_pass"
            };

            var user = new User
            {
                ResetPasswordExpires = new DateTime(2009, 12, 1)
            };

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult(user));

            _httpContextAccessor.Setup(x =>
                    x.HttpContext.Request.Query)
                .Returns(() => new QueryCollection());

            var result = await _resetPasswordCommandHandler.Handle(resetPasswordCommand, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.InvalidCode);
        }


        [Test]
        public async Task Authorization_ResetPassword_ResetPasswordSuccess()
        {
            var resetPasswordCommand = new ResetPasswordCommand
            {
                Password = "test_pass"
            };

            var user = new User
            {
                ResetPasswordExpires = DateTime.Now.AddDays(1)
            };

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult(user));

            _httpContextAccessor.Setup(x =>
                    x.HttpContext.Request.Query)
                .Returns(() => new QueryCollection());

            _userRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<Expression<Func<User, bool>>>()));

            var result = await _resetPasswordCommandHandler.Handle(resetPasswordCommand, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.ResetPasswordSuccess);
        }
    }
}