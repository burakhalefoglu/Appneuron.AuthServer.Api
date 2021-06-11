using System;
using Core.Entities;
using System.Collections.Generic;

#nullable disable

namespace Entities.Concrete
{
    public class UserProject : IEntity
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string ProjectKey { get; set; }
    }
}
