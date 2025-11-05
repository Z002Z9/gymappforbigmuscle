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
using api.Models;
using api.Repository;


namespace api.Controllers
{
    [ApiController]
    [Route("api/generateworkout")]
    public class WorkoutController : ControllerBase
    {
         
        private readonly ApplicationDBContext _context;

        private readonly IUserRepository _userRepository;

        private readonly IDailydataRepository _dailydataRepository;


        public WorkoutController(IUserRepository userRepository, IDailydataRepository dailydataRepository, ApplicationDBContext context)
        {
            _userRepository = userRepository;
            _dailydataRepository = dailydataRepository;
            _context = context;
        }
        

        [HttpGet("workoutgenerator")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GenerateWorkout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId == null)
                return Unauthorized("You must be logged in");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            
            Dailydata closestData = await _dailydataRepository.GetClosestDailyDataForUserAsync(userId);
            List<Exercise> workoutPlan = new List<Exercise>();
            string message = "";

            
            List<string> criticalInjuries = new List<string> 
            { 
                "váll", "könyök", "csukló", "alsóhát", "térdek", "boka" 
            };
            
            if (user.Injury.Count >= 4 || user.Injury.All(i => criticalInjuries.Contains(i)))
            {
                return Ok(new { Message = "You have too many injuries to train safely. Please consult a medical professional." });
            }

            switch (user.Trainingtype.ToLower())
            {
                case "ppl":
                    workoutPlan = GeneratePPLWorkout(closestData.Trainingdaytype, user.Injury);
                    break;
                case "fullbody":
                    workoutPlan = GenerateFullBodyWorkout(user.Injury);
                    break;
                case "upper-lower":
                    workoutPlan = GenerateUpperLowerWorkout(closestData.Trainingdaytype, user.Injury);
                    break;
            }

            
            if (workoutPlan.Count < 5)
            {
                var compensationExercises = GetCompensationExercises(user.Injury, 5 - workoutPlan.Count);
                workoutPlan.AddRange(compensationExercises);
            }

            return Ok(new { Exercises = workoutPlan, Message = message });
        }

        private List<Exercise> GeneratePPLWorkout(string dayType, List<string> injuries)
        {
            List<string> targetMuscles = new List<string>();
            
            switch (dayType.ToLower())
            {
                case "push":
                    targetMuscles = new List<string> { "back", "biceps", "shoulders" };
                    break;
                case "pull":
                    targetMuscles = new List<string> { "quads", "hamstrings", "calves", "glutes" };
                    break;
                case "leg":
                    targetMuscles = new List<string> { "chest", "shoulders", "triceps" };
                    break;
            }

            return GetAppropriateExercises(targetMuscles, injuries);
        }

        private List<Exercise> GenerateFullBodyWorkout(List<string> injuries)
        {
            var targetMuscles = new List<string> 
            { 
                "back", "biceps", "shoulders", "quads", "hamstrings", "calves", "glutes","chest", "shoulders", "triceps"
            };
            
            return GetAppropriateExercises(targetMuscles, injuries);
        }

        private List<Exercise> GenerateUpperLowerWorkout(string dayType, List<string> injuries)
        {
            List<string> targetMuscles = new List<string>();
            
            if (dayType.ToLower() == "upper")
            {
                targetMuscles = new List<string> { "quads", "hamstrings", "calves", "glutes", "core" };
            }
            else
            {
                targetMuscles = new List<string> { "chest", "back", "shoulders", "biceps", "triceps" };
            }

            return GetAppropriateExercises(targetMuscles, injuries);
        }

        private List<Exercise> GetAppropriateExercises(List<string> targetMuscles, List<string> injuries)
        {
            List<Exercise> exercises = new List<Exercise>();
            
            foreach (var muscle in targetMuscles)
            {
                var muscleExercises = _context.Exercises
                    .Where(e => e.AffectedBodyParts.Contains(muscle) && 
                        !e.Bannedexercise &&
                        !injuries.Any(i => e.AffectedBodyParts.Contains(i)))
                    .Take(2)
                    .ToList();
                    
                exercises.AddRange(muscleExercises);
            }

            
            if (exercises.Count > 6)
            {
                exercises = exercises.OrderBy(x => Guid.NewGuid()).Take(6).ToList();
            }

            return exercises;
        }

        private List<Exercise> GetCompensationExercises(List<string> injuries, int count)
        {
            
            return _context.Exercises
                .Where(e => !e.Bannedexercise && 
                    !injuries.Any(i => e.AffectedBodyParts.Contains(i)))
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToList();
        }
        

    } 
}