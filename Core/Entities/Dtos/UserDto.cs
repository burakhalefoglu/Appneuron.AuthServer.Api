namespace Core.Entities.Dtos
{
    public class UserDto : IDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobilePhones { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
    }
}