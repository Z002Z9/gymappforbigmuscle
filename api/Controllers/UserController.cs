using api.Data;
using api.Dtos.Dailydata;
using api.Interfaces;
using api.Mappers;
using api.Models;
using gymappforbigmuscle.Dtos.User;
using gymappforbigmuscle.Interfaces;
using gymappforbigmuscle.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace gymappforbigmuscle.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IUserRepository _userRepo;
        public UserController(ApplicationDBContext context, IUserRepository userRepo)
        {
            _userRepo = userRepo;
            _context = context;
        }

        [HttpGet("ListAllUser")]
        [Authorize(Roles = "1")]

        public async Task<IActionResult> GetAll()
        {

            var users = await _userRepo.GetAllAsync();

            var userDto = users.Select(s => s.ToUserDto());


            return Ok(userDto);
        }


        [HttpGet("ListUserByID/{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.ToUserDto());
        }
        [HttpPost("Registration")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userDto)
        {

            var userModel = userDto.ToUserFromCreateDto();

            var hasher = new PasswordHasher<User>();
            userModel.Password = hasher.HashPassword(userModel, userModel.Password);

            await _userRepo.CreateAsync(userModel);
            return CreatedAtAction(nameof(GetById), new { id = userModel.Id }, userModel.ToUserDto());
        }

        [HttpPut("EditUserByID/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto updateDto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            
            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, updateDto.Password);
            }

            user.Name = updateDto.Name; 
            user.Email = updateDto.Email;
            user.Age = updateDto.Age;
            user.Height = updateDto.Height;
            user.Weight = updateDto.Weight;
            user.Gender = updateDto.Gender;
            user.Trainingsperweek = updateDto.Trainingsperweek;
            user.Injury = updateDto.Injury;
            user.Allergys = updateDto.Allergys;
            user.Trainingtype = updateDto.Trainingtype;
            user.Kcalintake = updateDto.Kcalintake;

            await _context.SaveChangesAsync();

            return Ok(user.ToUserDto());
        }


        [HttpDelete("DeleteUserByID/{id}")]
        //ha egyszerre van httpdelete, majd benne egy �tvonal le�r�s("DeleteUserByID") �s k�l�n egy route, akkor api hib�t okoz
        [Authorize(Roles = "1")] //felt�telezz�k a user nem akarja t�r�lni mag�t?

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userModel = await _userRepo.DeleteAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }


            return NoContent();
        }
    }
}