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
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> CalculateCalories()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            if (userId == null)
                return Unauthorized("You must be logged in");


            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return NotFound("User not found");

            double bmr;
            if (string.Equals(user.Gender, "Férfi", StringComparison.OrdinalIgnoreCase))
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;
            else
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161;



            double activityFactor = 0;
            if (user.Trainingsperweek == 0)
            {
                activityFactor = 1.2;
            }
            else if (user.Trainingsperweek == 1 || user.Trainingsperweek == 2)
            {
                activityFactor = 1.375;
            }
            else if (user.Trainingsperweek == 3 || user.Trainingsperweek == 4)
            {
                activityFactor = 1.55;
            }
            else if (user.Trainingsperweek == 5 || user.Trainingsperweek == 6)
            {
                activityFactor = 1.725;
            }
            else if (user.Trainingsperweek >= 7)
            {
                activityFactor = 1.9;
            }

            var maintenanceCalories = bmr * activityFactor;

            int tmpkcal = Convert.ToInt32(Math.Round(maintenanceCalories));

            if (user.Goal == "Tömegelés")
            {
                tmpkcal += 500;
            }
            else if (user.Goal=="Fogyás")
            {
                tmpkcal -= 500;
            }
            

            user.Kcalintake = tmpkcal;
            await _context.SaveChangesAsync();


            return Ok(new { DailyCalories = tmpkcal });
        }

        [HttpGet("dailymacrosget")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Dailymacrosget()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            
            if (userId == null)
                return Unauthorized("You must be logged in");           
            

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return NotFound("User not found");

           

            List<int> macros=new List<int>();
            //protein,zsir,szénhidrát
            double kcalintake=user.Kcalintake;

            macros.Add((int)Math.Ceiling(kcalintake * 0.3 / 4)); //protein
            
            
            macros.Add((int)Math.Floor(kcalintake * 0.25 / 9));// zsir

            double tmp1=kcalintake -(kcalintake * 0.3) - (kcalintake * 0.25);
            
            macros.Add((int)Math.Floor(tmp1 / 4)); //szénhidrát

            //meg at kell valtani grammra


            return Ok(new { DailyMacros = macros });
        }
    }
}
