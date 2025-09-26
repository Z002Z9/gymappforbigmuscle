using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gymappforbigmuscle.Dtos.User
{
    public class UpdateUserRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Age { get; set; }

        public int Height { get; set; }

        public List<int> Injury { get; set; } = new List<int>();

        public List<string> Allergys { get; set; } = new List<string>();

        public int Kcalintake { get; set; }

        public string Trainingtype { get; set; } = string.Empty;

        public int Trainingsperweek { get; set; }

        public int Weight { get; set; }
        
        public string Gender { get; set; } = string.Empty;
    }
}