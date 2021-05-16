using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class Language : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Translate> Translates { get; set; }

    }
}