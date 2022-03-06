using System;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Mail;
using Core.Utilities.Results;
using Core.Utilities.Security.Encyption;
using DataAccess.Abstract;
using MediatR;
using MimeKit;
using MimeKit.Text;

namespace Business.Handlers.Authorizations.Commands
{
    public class ForgotPasswordCommand : IRequest<IResult>
    {
        public string Email { get; set; }

        public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IResult>
        {
            private readonly IMailService _mailService;
            private readonly IUserRepository _userRepository;

            public ForgotPasswordCommandHandler(IUserRepository userRepository,
                IMailService mailService)
            {
                _userRepository = userRepository;
                _mailService = mailService;
            }

            /// <summary>
            /// </summary>
            /// <param name="request"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            [PerformanceAspect(5)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [TransactionScopeAspect]
            public async Task<IResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(u => u.Email == request.Email && u.Status);

                if (user == null)
                    // if is not email do noting. But return success result to not give database information !!!
                    return new SuccessResult(Messages.SendPassword);

                var token = SecurityKeyHelper.GetRandomHexNumber(64);
                var url = "https://webapi.appneuron.net/auth/api/Auth/resetpassword?token=" + token;
                await _mailService.Send(new EmailMessage
                {
                    Content = new TextPart(TextFormat.Html)
                        {Text = $"<a href = '{url}'>reset your pass...</a>"},
                    FromAddresses =
                    {
                        new EmailAddress
                        {
                            Address = "info@appneuron.com",
                            Name = "Appneuron"
                        }
                    },
                    Subject = "Reset password Mail...",
                    ToAddresses =
                    {
                        new EmailAddress
                        {
                            Address = user.Email,
                            Name = user.Name
                        }
                    }
                });

                user.ResetPasswordToken = token.ToLower();
                user.ResetPasswordExpires = DateTime.Now.AddMinutes(10);
                await _userRepository.UpdateAsync(user);

                return new SuccessResult(Messages.SendPassword);
            }
        }
    }
}