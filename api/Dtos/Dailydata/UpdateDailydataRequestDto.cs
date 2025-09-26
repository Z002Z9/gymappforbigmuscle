using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Dailydata
{
    public class UpdateDailydataRequestDto
    {
        public DateOnly Date { get; set; } 
        public int Weight { get; set; }
        public int Dailykcalintake { get; set; }
        public bool Trainedtoday { get; set; } 
    }
}