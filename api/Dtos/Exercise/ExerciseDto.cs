using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Exercise
{
    public class ExerciseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Mainmuscle { get; set; } = string.Empty;
        public string Youtubelink { get; set; } = string.Empty;       
        public int Setnumber { get; set; }

        public int Repnumber { get; set; }

        public List<int> Injuryblacklist { get; set; } = new List<int>();        

        public bool Bannedexercise { get; set; } = false;
    }
}