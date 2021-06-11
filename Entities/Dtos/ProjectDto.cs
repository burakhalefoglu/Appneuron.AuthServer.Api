using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Entities.Dtos
{
    public class ProjectDto : IDto
    {
        public int UserId { get; set; }
        public string ProjectKey { get; set; }
    }
}
