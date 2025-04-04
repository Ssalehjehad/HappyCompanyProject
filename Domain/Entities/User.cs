using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public bool Active { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
