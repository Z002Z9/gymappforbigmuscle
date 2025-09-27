using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using gymappforbigmuscle.Dtos.User;

namespace gymappforbigmuscle.Mappers
{
    public static class UserMappers
    {
        public static UserDto ToUserDto(this User userModel)
        {
            return new UserDto
            {
                Id= userModel.Id,
                Name = userModel.Name,
                Email = userModel.Email,
                Password = userModel.Password,
                RoleId = userModel.RoleId,                
                Age = userModel.Age,
                Height = userModel.Height,
                Injury = userModel.Injury,
                Allergys = userModel.Allergys,
                Kcalintake = userModel.Kcalintake,
                Trainingtype = userModel.Trainingtype,
                Trainingsperweek = userModel.Trainingsperweek,
                Weight = userModel.Weight,
                Gender = userModel.Gender
            };
        }

        public static User ToUserFromCreateDto(this CreateUserRequestDto userDto)
        { 
            return new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                RoleId = userDto.RoleId  
                
            };


        }
    }
}