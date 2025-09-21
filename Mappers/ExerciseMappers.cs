using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Dtos.Exercise;
using api.Models; 


namespace api.Mappers
{
    public static class ExerciseMappers
    {
        public static ExerciseDto ToExerciseDto(this Exercise exerciseModel)
        {
            return new ExerciseDto
            {
                Name = exerciseModel.Name,
                Mainmuscle = exerciseModel.Mainmuscle,
                Youtubelink = exerciseModel.Youtubelink,
                Setnumber = exerciseModel.Setnumber,
                Repnumber = exerciseModel.Repnumber,
                Injuryblacklist = exerciseModel.Injuryblacklist,
                Bannedexercise = exerciseModel.Bannedexercise
            };
        }

        public static Exercise ToExerciseFromCreateDto(this CreateExerciseRequestDto exerciseDto)
        { 
            return new Exercise
            {                
                Name = exerciseDto.Name,
                Mainmuscle = exerciseDto.Mainmuscle,
                Youtubelink = exerciseDto.Youtubelink,
                Setnumber = exerciseDto.Setnumber,
                Repnumber = exerciseDto.Repnumber                   
                
            };


        }
    }
}