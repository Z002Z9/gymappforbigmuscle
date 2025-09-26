using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gymappforbigmuscle.Dtos.User
{
    public class CreateUserRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? RoleId { get; set; }    
        
    }
}