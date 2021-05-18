﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Helpers
{
    public static class UserNameCreationHelper
    {
        public static string EmailToUsername(string email)
        {
            StringBuilder bld = new StringBuilder();



            foreach (var item in email)
            {
                if (item.ToString() == "@")
                {
                    break;
                }
                bld.Append(item);

            }
            string username = bld.ToString();

            return AppendRandomNumber(username);

        }

        private static string AppendRandomNumber(string username)
        {
            Random random = new Random();
            var number = random.Next(11111111, 999999999);

            return username + number;
        }
    }
}
