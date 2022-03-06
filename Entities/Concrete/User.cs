using Core.Entities;

namespace Entities.Concrete
{
    public class User : IEntity
    {
        public User()
        {
            UpdateContactDate = RecordDate = DateTimeOffset.Now;
            ResetPasswordExpires = DateTimeOffset.MinValue;
            Status = true;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public DateTimeOffset RecordDate { get; set; }
        public DateTimeOffset UpdateContactDate { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTimeOffset ResetPasswordExpires { get; set; }
    }
}