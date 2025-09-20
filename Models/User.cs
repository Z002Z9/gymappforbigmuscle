using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        //ez osszekoti a rolelal
        public int? RoleId { get; set; }
        //ez meg kell hogy a roleon bellul tudjunk navigalni
        public Role? Role { get; set; }
        public int Age { get; set; }

        public int Height { get; set; }

        public List<int> Injury { get; set; } = new List<int>();

        public List<string> Allergys { get; set; } = new List<string>();

        public int Kcalintake { get; set; }

        public string Trainingtype { get; set; } = string.Empty;

        public int Trainingsperweek { get; set; }
        

    }
}