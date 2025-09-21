using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Models; 

namespace api.Mappers
{
    public static class DailydataMappers
    {
        public static DailydataDto ToDailydataDto(this Dailydata dailydataModel)
        {
            return new DailydataDto
            {
                Date = dailydataModel.Date,
                Weight = dailydataModel.Weight,
                Dailykcalintake = dailydataModel.Dailykcalintake,
                Trainedtoday = dailydataModel.Trainedtoday
            };
        }

        public static Dailydata ToDailydataFromCreateDto(this CreateDailydataRequestTrueDto dailydataDto)
        { 
            return new Dailydata
            {                
                Weight = dailydataDto.Weight,
                Dailykcalintake = dailydataDto.Dailykcalintake                
                
            };


        }
    }    

}