using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class Language : DocumentDbEntity
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public bool Status = true;
    }
}