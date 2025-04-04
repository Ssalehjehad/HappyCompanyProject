
namespace Application.DTOs.User
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public bool Active { get; set; }
        public string Password { get; set; }
    }
}
