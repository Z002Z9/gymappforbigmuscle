using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Dtos.Trainingprogram;
using api.Models;  

namespace api.Mappers
{
    public static class TrainingprogramMappers
    {
         public static TrainingprogramDto ToTrainingprogramDto(this Trainingprogram trainingprogramModel)
        {
            return new TrainingprogramDto
            {
                Name = trainingprogramModel.Name,
                Plan = trainingprogramModel.Plan,
                Daysperweek = trainingprogramModel.Daysperweek,
                Kcalburned = trainingprogramModel.Kcalburned
            };
        }

        public static Trainingprogram ToTrainingprogramFromCreateDto(this CreateTrainingprogramRequestDto trainingprogramDto)
        { 
            return new Trainingprogram
            {                
                Name = trainingprogramDto.Name,
                Plan = trainingprogramDto.Plan,
                Daysperweek = trainingprogramDto.Daysperweek,
                Kcalburned = trainingprogramDto.Kcalburned            
                
            };


        }
    }
}