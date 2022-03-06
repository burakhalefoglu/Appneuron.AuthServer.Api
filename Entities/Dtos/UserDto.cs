using Core.Entities;

namespace Entities.Dtos
{
    public class UserDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
    }
}