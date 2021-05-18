using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.ClaimModels
{
    public class UserClaimModel
    {
        public int UserId { get; set; }
        public string UniqueKey { get; set; }
        public string[] OperationClaims { get; set; }
    }
}
