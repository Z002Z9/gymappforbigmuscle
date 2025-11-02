using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Dailydata
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int Weight { get; set; }
        public int Dailykcalintake { get; set; }
        public bool Trainedtoday { get; set; } = true;

        public string Trainingdaytype { get; set; } = string.Empty;
        
        public int UserId { get; set; }
        public User? User { get; set; }

    }
}