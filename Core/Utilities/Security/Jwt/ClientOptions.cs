using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Security.Jwt
{
    public class ClientOptions
    {
        public string Audience { get; set; }
        public long AccessTokenExpiration { get; set; }
    }
}
