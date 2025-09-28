using Microsoft.AspNetCore.Mvc;
using api.Interfaces;
using System.Security.Claims;
using gymappforbigmuscle.Interfaces;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KcalcalculatorController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        

        public KcalcalculatorController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            
            return Ok(new { DailyCalories = maintenanceCalories });
        }
    }
}
