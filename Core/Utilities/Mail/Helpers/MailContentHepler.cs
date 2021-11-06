using Core.Entities.Concrete;
using System.IO;

namespace Core.Utilities.Mail.Helpers
{
    public static class MailContentHepler
    {
        public static string GetResetMailContent(User user, string token)
        {
            string MailText = ReadMailText("ResetMail.html");
            MailText = MailText.Replace("[Link]", "https://localhost:44375/WebAPI/api/Auth/resetpassword?token=" + token)
                .Replace("[Name]", user.Name);
            return MailText;
        }

        private static string ReadMailText(string templateName)
        {
            string FilePath = Path.GetFullPath(Path.Combine(@"..\..\..\..\" +"\\Core\\Utilities\\Mail\\MailTemplates\\" + templateName));

            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            return MailText;
        }
    }
}