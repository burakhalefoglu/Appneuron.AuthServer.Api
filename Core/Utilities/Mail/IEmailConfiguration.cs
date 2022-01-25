namespace Core.Utilities.Mail
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        string SmtpPort { get; }
        string SmtpUserName { get; }
        string Password { get; }
    }
}