using Business.Constants;
using Business.Handlers.Authorizations.Commands;
using Business.Handlers.Authorizations.Queries;
using Core.CrossCuttingConcerns.Caching;
using Core.Entities.Concrete;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Fakes.Handlers.GroupClaims;
using Business.Handlers.UserProjects.Queries;
using Business.Services.Authentication;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using Entities.Concrete;
using Tests.Helpers;
using static Business.Handlers.Authorizations.Commands.ForgotPasswordCommand;
using static Business.Handlers.Authorizations.Commands.RegisterUserCommand;
using static Business.Handlers.Authorizations.Commands.ResetPasswordCommand;
using static Business.Handlers.Authorizations.Queries.LoginUserQuery;
using Core.Entities.ClaimModels;
using Core.Utilities.Mail;
using Microsoft.AspNetCore.Http;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class AuthorizationsTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<ITokenHelper> _tokenHelper;
        private Mock<IMediator> _mediator;
        private Mock<ICacheManager> _cacheManager;
        private Mock<IMailService> _mailService;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private LoginUserQueryHandler _loginUserQueryHandler;
        private LoginUserQuery _loginUserQuery;
        private RegisterUserCommandHandler _registerUserCommandHandler;
        private RegisterUserCommand _command;
        private ForgotPasswordCommandHandler _forgotPasswordCommandHandler;
        private ForgotPasswordCommand _forgotPasswordCommand;
        private ResetPasswordCommandHandler _resetPasswordCommandHandler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _tokenHelper = new Mock<ITokenHelper>();
            _mediator = new Mock<IMediator>();
            _cacheManager = new Mock<ICacheManager>();
            _mailService = new Mock<IMailService>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _loginUserQueryHandler = new LoginUserQueryHandler(_userRepository.Object,
                _tokenHelper.Object,
                _mediator.Object,
                _cacheManager.Object);

            _registerUserCommandHandler = new RegisterUserCommandHandler(_userRepository.Object,
                _mediator.Object, _tokenHelper.Object, _cacheManager.Object);

            _forgotPasswordCommandHandler = new ForgotPasswordCommandHandler(_userRepository.Object, 
                _mailService.Object);

            _resetPasswordCommandHandler = new ResetPasswordCommandHandler(_userRepository.Object,
                _mediator.Object,
                _httpContextAccessor.Object);

        }

        [Test]
        public async Task Handler_Login_UserNotFount()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            _userRepository.
                            Setup(x =>
                                x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).
                            Returns(Task.FromResult<User>(null));

            _userRepository.Setup(x => x.GetClaims(It.IsAny<int>()))
                            .Returns(new List<OperationClaim>() { new() { Id = 1, Name = "test" } });
            _loginUserQuery = new LoginUserQuery
            {
                Email = user.Email,
                Password = "123456"
            };

            var result = await _loginUserQueryHandler.Handle(_loginUserQuery, new System.Threading.CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.UserNotFound);
        }


        [Test]
        public async Task Handler_Login_PasswordError()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _userRepository.
                Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).
                Returns(() => Task.FromResult(user));

            _userRepository.Setup(x => x.GetClaims(It.IsAny<int>()))
                .Returns(new List<OperationClaim>() { new() { Id = 1, Name = "test" } });
            _loginUserQuery = new LoginUserQuery
            {
                Email = user.Email,
                Password = "1234567"
            };

            var result = await _loginUserQueryHandler.Handle(_loginUserQuery, new System.Threading.CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.PasswordError);
        }


        [Test]
        public async Task Handler_Login_Success()
        {
            var user = DataHelper.GetUser("test");
            HashingHelper.CreatePasswordHash("123456",
                out var passwordSalt,
                out var passwordHash);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _userRepository.
                Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).
                Returns(() => Task.FromResult(user));


            _mediator.Setup(m =>
                m.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<SelectionItem>>(new List<SelectionItem>()
                    {
                        new SelectionItem()
                        {
                            Id = 1,
                            Label = "test",
                            IsDisabled = false,
                            ParentId = "test"

                        }
                    }));

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetUserProjectsInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<UserProject>>(new List<UserProject>()
                    {
                        new UserProject()
                        {
                            Id = 1,
                          ProjectKey = "sdfsdfds",
                          UserId = 1,

                        }
                    }));


            _tokenHelper.Setup(x => x.CreateCustomerToken<DArchToken>(new UserClaimModel
            {
                UserId = user.UserId,
                OperationClaims = null,
            }, new List<string>())).Returns(new DArchToken());


            _loginUserQuery = new LoginUserQuery
            {
                Email = user.Email,
                Password = "123456"
            };

            var result = await _loginUserQueryHandler.Handle(_loginUserQuery, new System.Threading.CancellationToken());

            result.Success.Should().BeTrue();
        }


        [Test]
        public async Task Handler_Register_EmailAlreadyExist()
        {
            var registerUser = new User { Email = "test@test.com", Name = "test test" };
            _command = new RegisterUserCommand
            {
                Email = registerUser.Email,
                Password = "123456"
            };

            _userRepository.Setup((x =>
                    x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())))
                .Returns(Task.FromResult(registerUser));


            var result = await _registerUserCommandHandler.Handle(_command, new System.Threading.CancellationToken());

            result.Message.Should().Be(Messages.EmailAlreadyExist);
        }


        [Test]
        public async Task Handler_Register_SuccessfulLogin()
        {
            var registerUser = new User {UserId = 1, Email = "test@test.com", Name = "test test" };
            _command = new RegisterUserCommand
            {
                Email = registerUser.Email,
                Password = "123456",
            };

            _userRepository.Setup((x =>
                    x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())))
                .Returns(Task.FromResult<User>(null));

            _userRepository.Setup(x => x.Add(It.IsAny<User>()))
                .Returns(registerUser);

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetGroupClaimsLookupByGroupIdInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<SelectionItem>>(new List<SelectionItem>()
                    {
                        new SelectionItem()
                        {
                            Id = 1,
                            Label = "test",
                            IsDisabled = false,
                            ParentId = "test"

                        }
                    }));

            _mediator.Setup(m =>
                    m.Send(It.IsAny<GetUserProjectsInternalQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new SuccessDataResult<IEnumerable<UserProject>>(new List<UserProject>()
                    {
                        new UserProject()
                        {
                            Id = 1,
                            ProjectKey = "sdfsdfds",
                            UserId = 1,

                        }
                    }));

            _tokenHelper.Setup(x => x.CreateCustomerToken<DArchToken>(new UserClaimModel
            {
                UserId = registerUser.UserId,
                OperationClaims = null,
            }, new List<string>())).Returns(new DArchToken());


            var result = await _registerUserCommandHandler.Handle(_command, new System.Threading.CancellationToken());

            result.Message.Should().Be(Messages.SuccessfulLogin);
        }


        [Test]
        public async Task Handler_ForgotPassword_WrongEmail()
        {
            var user = DataHelper.GetUser("test");
            _userRepository.
                                    Setup(x =>
                                        x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                                    .Returns(() => Task.FromResult<User>(null));

            _forgotPasswordCommand = new ForgotPasswordCommand
            {
                Email = user.Email,
            };
            var result = await _forgotPasswordCommandHandler.Handle(_forgotPasswordCommand, new System.Threading.CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.WrongEmail);
        }
        
        [Test]
        public async Task Handler_ForgotPassword_Success()
        {
            var user = DataHelper.GetUser("test");
            _userRepository.
                                    Setup(x => 
                                        x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                                    .Returns(() => Task.FromResult(user));

            _forgotPasswordCommand = new ForgotPasswordCommand
            {
                Email = user.Email,
            };

            _mailService.Setup(x => x.Send(It.IsAny<EmailMessage>()));

            _userRepository.Setup(x => x.Update(It.IsAny<User>()));

            var result = await _forgotPasswordCommandHandler.Handle(_forgotPasswordCommand, new System.Threading.CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.SendPassword);
        }


        [Test]
        public async Task Handler__ResetPassword_InvalidCode()
        {
            var resetPasswordCommand = new ResetPasswordCommand()
            {
                Password = "fsdfjsdfkhsdlf"
            };

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult<User>(null));

            _httpContextAccessor.Setup(x => 
                x.HttpContext.Request.Query)
                .Returns(() => new QueryCollection()
                {});

            var result = await _resetPasswordCommandHandler.Handle(resetPasswordCommand, new System.Threading.CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.InvalidCode);
        }

        [Test]
        public async Task Handler__ResetPassword_InvalidCodeWhenResPasExpiresLowerThanDatetimeNow()
        {
            var resetPasswordCommand = new ResetPasswordCommand()
            {
                Password = "fsdfjsdfkhsdlf"
            };

            var user = new User()
            {
                ResetPasswordExpires = new DateTime(2009,12,1)
            };

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult<User>(user));

            _httpContextAccessor.Setup(x =>
                    x.HttpContext.Request.Query)
                .Returns(() => new QueryCollection()
                    { });

            var result = await _resetPasswordCommandHandler.Handle(resetPasswordCommand, new System.Threading.CancellationToken());
            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.InvalidCode);
        }


        [Test]
        public async Task Handler__ResetPassword_ResetPasswordSuccess()
        {
            var resetPasswordCommand = new ResetPasswordCommand()
            {
                Password = "fsdfjsdfkhsdlf"
            };

            var user = new User()
            {
                ResetPasswordExpires = DateTime.Now.AddDays(1)
            };

            _userRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(() => Task.FromResult<User>(user));

            _httpContextAccessor.Setup(x =>
                    x.HttpContext.Request.Query)
                .Returns(() => new QueryCollection()
                    { });

            _userRepository.Setup(x => x.Update(It.IsAny<User>()));

            var result = await _resetPasswordCommandHandler.Handle(resetPasswordCommand, new System.Threading.CancellationToken());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.ResetPasswordSuccess);
        }
    }
}