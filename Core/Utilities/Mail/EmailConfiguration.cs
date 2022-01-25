namespace Core.Utilities.Mail
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }

        public string SmtpPort { get; set; }

        public string SmtpUserName { get; set; }

        public string Password { get; set; }
    }
}