using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Dailydata
{
    public class DailydataDto
    {
        //id removed
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int Weight { get; set; }
        public int Dailykcalintake { get; set; }
        public bool Trainedtoday { get; set; } = true;

        public string Trainingdaytype { get; set; } = string.Empty;
        
        public int UserId { get; set; }

    }
}