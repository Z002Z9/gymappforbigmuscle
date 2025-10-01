using Microsoft.AspNetCore.Mvc;
using api.Interfaces;
using System.Security.Claims;
using gymappforbigmuscle.Interfaces;
using api.Data;
using api.Dtos.Dailydata;
using api.Mappers;
using api.Models;
using gymappforbigmuscle.Dtos.User;
using gymappforbigmuscle.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KcalcalculatorController : ControllerBase
    {
        private readonly ApplicationDBContext _context; 
        
        private readonly IUserRepository _userRepository;


        public KcalcalculatorController(IUserRepository userRepository, ApplicationDBContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        [HttpGet("daily-calories")]
        public async Task<IActionResult> CalculateCalories()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            if (userId == null)
                return Unauthorized("You must be logged in");


            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return NotFound("User not found");

            double bmr;
            if (user.Gender?.ToLower() == "male")
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;
            else
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161;

            double activityFactor = user.Trainingsperweek switch
            {
                <= 2 => 1.2,
                <= 4 => 1.375,
                _ => 1.55
            };

            var maintenanceCalories = bmr * activityFactor;
            int tmpkcal=Convert.ToInt32(Math.Round(maintenanceCalories));
            user.Kcalintake = tmpkcal;
            await _context.SaveChangesAsync();


            return Ok(new { DailyCalories = tmpkcal });
        }
        
        [HttpGet("dailymacrosget")]
        public async Task<IActionResult> Dailymacrosget()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            
            if (userId == null)
                return Unauthorized("You must be logged in");           
            

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return NotFound("User not found");

           

            List<int> macros=new List<int>();
            //protein,zsir,szénhidrát
            
            macros.Add((int)Math.Ceiling(user.Kcalintake * 0.3)); 
            macros.Add((int)Math.Floor(user.Kcalintake * 0.25));
            macros.Add(user.Kcalintake - macros[0] - macros[1]);

            
            return Ok(new { DailyMacros = macros });
        }
    }
}
