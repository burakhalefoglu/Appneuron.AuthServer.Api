using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Concrete
{
    public class User : IEntity
    {
        public int UserId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string MobilePhones { get; set; }
        public bool Status { get; set; }
        public DateTime RecordDate { get; set; }
        public string Notes { get; set; }
        public DateTime UpdateContactDate { get; set; }

        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTime ResetPasswordExpires { get; set; }

        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<UserClaim> UserClaims { get; set; }




        public User()
        {
            UpdateContactDate = RecordDate = DateTime.Now;
            ResetPasswordExpires = DateTime.MinValue;
            Status = true;
        }

        public bool UpdateMobilePhone(string mobilePhone)
        {
            if (mobilePhone != MobilePhones)
            {
                MobilePhones = mobilePhone;
                return true;
            }
            else
                return false;
        }
    }
}