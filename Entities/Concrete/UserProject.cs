using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class UserProject : DocumentDbEntity
    {
        public string UserId { get; set; }
        public string ProjectKey { get; set; }
        public bool Status { get; set; }
    }
}