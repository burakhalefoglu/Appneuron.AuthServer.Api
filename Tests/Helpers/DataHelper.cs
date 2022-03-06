using System;
using System.Collections.Generic;
using Core.Entities.Concrete;
using Core.Utilities.Security.Hashing;
using Entities.Concrete;

namespace Tests.Helpers
{
    public static class DataHelper
    {
        public static User GetUser(string name)
        {
            HashingHelper.CreatePasswordHash("123456", out var passwordSalt, out var passwordHash);

            return new User
            {
                Email = "test@test.com",
                Name = $"{name} {name} {name}",
                RecordDate = DateTime.Now,
                PasswordHash = passwordSalt,
                PasswordSalt = passwordHash,
                Status = true,
                UpdateContactDate = DateTime.Now
            };
        }

        public static List<User> GetUserList()
        {
            HashingHelper.CreatePasswordHash("123456", out var passwordSalt, out var passwordHash);
            var list = new List<User>();

            for (var i = 1; i <= 5; i++)
            {
                var user = new User
                {
                    Email = "test@test.com",
                    Name = $"name {i} name {i} name {i}",
                    RecordDate = DateTime.Now,
                    PasswordHash = passwordSalt,
                    PasswordSalt = passwordHash,
                    Status = true,
                    UpdateContactDate = DateTime.Now
                };
                list.Add(user);
            }

            return list;
        }
    }
}