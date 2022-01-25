using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class Group : DocumentDbEntity
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public bool Status = true;
    }
}