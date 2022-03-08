using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Authorizations.Commands;
using Business.Internals.Handlers.GroupClaims;
using Business.Internals.Handlers.UserGroups.Queries;
using Core.Entities.ClaimModels;
using Core.Utilities.Mail;
using Core.Utilities.MessageBrokers;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Tests.Helpers;
using static Business.Handlers.Authorizations.Commands.ForgotPasswordCommand;
using static Business.Handlers.Authorizations.Commands.ResetPasswordCommand;
using static Business.Handlers.Authorizations.Commands.LoginOrRegisterUserCommand;

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
            _messageBroker = new Mock<IMessageBroker>();
            
            _loginUserQueryHandler = new LoginOrRegisterUserCommand.LoginOrRegisterUserCommandHandler(_userRepository.Object,
                _tokenHelper.Object,
                _mediator.Object);
            
            _forgotPasswordCommandHandler = new ForgotPasswordCommandHandler(_userRepository.Object,
                _mailService.Object);

            _resetPasswordCommandHandler = new ResetPasswordCommandHandler(_userRepository.Object,
                _httpContextAccessor.Object);
        }

        private Mock<IUserRepository> _userRepository;
        private Mock<ITokenHelper> _tokenHelper;
        private Mock<IMediator> _mediator;
        private Mock<IMailService> _mailService;
        private Mock<IMessageBroker> _messageBroker;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private LoginOrRegisterUserCommandHandler _loginUserQueryHandler;
        private LoginOrRegisterUserCommand _loginUserQuery;
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
                    m.Send(It.IsAny<GetGroupClaimInternalQuery>(),
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
            _loginUserQuery = new LoginOrRegisterUserCommand
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
                    m.Send(It.IsAny<GetGroupClaimInternalQuery>(),
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

            _loginUserQuery = new LoginOrRegisterUserCommand
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
                Id = 1
            }));

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetGroupClaimInternalQuery>(),
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

            _tokenHelper.Setup(x => x.CreateCustomerToken<AccessToken>(new UserClaimModel
            {
                OperationClaims = null
            }, "test@email.com")).Returns(new AccessToken());


            _loginUserQuery = new LoginOrRegisterUserCommand
            {
                Email = user.Email,
                Password = "123456"
            };

            var result = await _loginUserQueryHandler.Handle(_loginUserQuery, new CancellationToken());

            result.Success.Should().BeTrue();
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

            _userRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()));

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

            _userRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()));

            var result = await _resetPasswordCommandHandler.Handle(resetPasswordCommand, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.ResetPasswordSuccess);
        }
    }
}