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
using System.Security.Cryptography.X509Certificates;




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

            string mytrainingDayType = "fullbody";//kell ertekadas mert kulonben hisztizik a code

            if (closestData == null)
            {

                switch (user.Trainingtype)
                {
                    case "ppl":
                        mytrainingDayType = "leg";
                        break;
                    case "upper-lower":
                        mytrainingDayType = "lower";
                        break;
                    case "fullbody":
                        mytrainingDayType = "fullbody";
                        break;
                }
            }
            else
            {

                mytrainingDayType = closestData.Trainingdaytype;
            }


            List<string> criticalInjuries = new List<string>
            {
                "váll", "könyök", "csukló", "alsóhát", "térdek", "boka"
            };


            if (user.Injury.Any(i => i == "váll" || i == "könyök" || i == "csukló") && user.Injury.Any(i => i == "térdek" || i == "boka" || i == "alsóhát"))
            {
                return Ok(new { Message = "You have too many injuries to train safely. Please consult a medical professional." });
            }

            switch (user.Trainingtype)
            {
                case "ppl":
                    workoutPlan = await GeneratePPLWorkout(mytrainingDayType, user.Injury,userId);
                    break;
                case "fullbody":
                    workoutPlan = await GenerateFullBodyWorkout(user.Injury,userId);
                    break;
                case "upper-lower":
                    workoutPlan = await GenerateUpperLowerWorkout(mytrainingDayType, user.Injury,userId);
                    break;
            }


            return Ok(new { Exercises = workoutPlan, Message = message });
        }

        private async Task<List<Exercise>> GeneratePPLWorkout(string dayType, List<string> injuries, int userId)
        {
            //"váll", "könyök", "csukló", "alsóhát", "térdek", "boka" 
            List<string> targetMuscles = new List<string>();
            if (dayType == "leg" && !injuries.Any(i => i == "váll" || i == "könyök" || i == "csukló"))
            {
                //push
                targetMuscles = new List<string> { "chest", "shoulders", "triceps" };
                await UpdateOrCreateTodayDailydataAsync(userId, "push");
            }
            else if (dayType == "push" && !injuries.Any(i => i == "váll" || i == "könyök" || i == "csukló"))
            {
                //pull
                targetMuscles = new List<string> { "back", "biceps", "shoulders" };
                await UpdateOrCreateTodayDailydataAsync(userId, "pull");
            }
            else if (dayType == "pull" && !injuries.Any(i => i == "térdek" || i == "boka" || i == "alsóhát"))
            {
                //leg
                targetMuscles = new List<string> { "quads", "hamstrings", "calves", "glutes" };
                await UpdateOrCreateTodayDailydataAsync(userId, "leg");
            }
            else if (dayType == "push" || dayType == "leg" && injuries.Any(i => i == "váll" || i == "könyök" || i == "csukló") && !injuries.Any(i => i == "térdek" || i == "boka" || i == "alsóhát"))
            {
                //leg mert mashoz serult
                targetMuscles = new List<string> { "quads", "hamstrings", "calves", "glutes" };
                await UpdateOrCreateTodayDailydataAsync(userId, "leg");
            }
            else if (dayType == "pull" && injuries.Any(i => i == "térdek" || i == "boka" || i == "alsóhát") && !injuries.Any(i => i == "váll" || i == "könyök" || i == "csukló"))
            {
                //push or pull
                int bit = Random.Shared.Next(0, 2);
                if (bit == 0)
                {
                    targetMuscles = new List<string> { "back", "biceps", "shoulders" };
                    await UpdateOrCreateTodayDailydataAsync(userId, "pull");
                }
                else
                {
                    targetMuscles = new List<string> { "chest", "shoulders", "triceps" };
                    await UpdateOrCreateTodayDailydataAsync(userId, "push");
                }
            }
            else if (!injuries.Any(i => i == "váll" || i == "könyök" || i == "csukló"))
            {
                targetMuscles = new List<string> { "chest", "shoulders", "triceps" };
                await UpdateOrCreateTodayDailydataAsync(userId, "push");
            }
            else
            {
                targetMuscles = new List<string> { "quads", "hamstrings", "calves", "glutes" };
                await UpdateOrCreateTodayDailydataAsync(userId, "leg");
            }

            return GetAppropriateExercises(targetMuscles, injuries, new List<Exercise>());
        }

        private async Task<List<Exercise>> GenerateFullBodyWorkout(List<string> injuries, int userId)
        {
            var targetMuscles = new List<string>
            {
                "back", "biceps", "shoulders", "quads", "hamstrings", "calves", "glutes","chest", "triceps"
            };

            await UpdateOrCreateTodayDailydataAsync(userId, "fullbody");

            return GetAppropriateExercises(targetMuscles, injuries, new List<Exercise>());
        }

        private async Task<List<Exercise>> GenerateUpperLowerWorkout(string dayType, List<string> injuries, int userId)
        {
            List<string> targetMuscles = new List<string>();

            if (dayType == "upper" && !injuries.Any(i => i == "térdek" || i == "boka" || i == "alsóhát"))
            {
                targetMuscles = new List<string> { "quads", "hamstrings", "calves", "glutes", "core" };
                await UpdateOrCreateTodayDailydataAsync(userId, "lower");
            }
            else if (dayType == "lower" && !injuries.Any(i => i == "váll" || i == "könyök" || i == "csukló"))
            {
                targetMuscles = new List<string> { "chest", "back", "shoulders", "biceps", "triceps" };
                await UpdateOrCreateTodayDailydataAsync(userId, "upper");
            }
            else if (!injuries.Any(i => i == "térdek" || i == "boka" || i == "alsóhát"))
            {
                targetMuscles = new List<string> { "quads", "hamstrings", "calves", "glutes", "core" };
                await UpdateOrCreateTodayDailydataAsync(userId, "lower");
            }
            else
            {
                targetMuscles = new List<string> { "chest", "back", "shoulders", "biceps", "triceps" };
                await UpdateOrCreateTodayDailydataAsync(userId, "upper");
            }

            return GetAppropriateExercises(targetMuscles, injuries, new List<Exercise>());
        }

        private List<Exercise> GetAppropriateExercises(List<string> targetMuscles, List<string> injuries, List<Exercise> alreadyChosen)
        {
            List<Exercise> exercises = new List<Exercise>();


            var alreadyChosenIds = alreadyChosen.Select(e => e.Id).ToList();

            foreach (var muscle in targetMuscles)
            {
                var muscleExercises = _context.Exercises                 
                    .Where(e => e.Mainmuscle == muscle && 
                        !e.Bannedexercise &&
                        !injuries.Any(i => e.AffectedBodyParts.Contains(i)) &&
                        !alreadyChosenIds.Contains(e.Id) 
                        )
                    .OrderBy(x => Guid.NewGuid()) 
                    .Take(2) 
                    .ToList();

                exercises.AddRange(muscleExercises);


                alreadyChosenIds.AddRange(muscleExercises.Select(e => e.Id));
            }

            if (exercises.Count > 6)
            {
                exercises = exercises.OrderBy(x => Guid.NewGuid()).Take(6).ToList();
            }
            if (exercises.Count < 5)
            {
                int missingCount = 5 - exercises.Count;
                var compensationExercises = GetCompensationExercises(injuries, missingCount, targetMuscles, exercises);
                exercises.AddRange(compensationExercises);
            }


            return exercises;
        }

        private List<Exercise> GetCompensationExercises(List<string> injuries, int count, List<string> musclestoworkout, List<Exercise> alreadychosen)
        {
            int N = musclestoworkout.Count;
            List<Exercise> exercises = new List<Exercise>();

            var idsToAvoid = alreadychosen.Select(e => e.Id).ToList();

            int randomNumber;
            for (int i = 0; i < count; i++)
            {
                if (musclestoworkout.Count == 0)
                    break;

                randomNumber = Random.Shared.Next(N);
                string targetMuscle = musclestoworkout[randomNumber];

                var chosenExercise = _context.Exercises
                    .Where(e => e.Mainmuscle == targetMuscle &&
                                !e.Bannedexercise &&
                                !injuries.Any(j => e.AffectedBodyParts.Contains(j)) &&
                                !idsToAvoid.Contains(e.Id)

                                )
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefault();

                if (chosenExercise != null)
                {
                    exercises.Add(chosenExercise);
                    idsToAvoid.Add(chosenExercise.Id);
                }
            }

            return exercises;
        }

         private async Task UpdateOrCreateTodayDailydataAsync(int userId, string trainingDayType)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);

                
                var existing = await _context.Dailydatas
                    .FirstOrDefaultAsync(d => d.UserId == userId && d.Date == today);

                if (existing != null)
                {
                    existing.Trainedtoday = true;
                    existing.Trainingdaytype = trainingDayType;
                    _context.Dailydatas.Update(existing);
                }
                else
                {
                    
                    var last = await _context.Users
                        .Where(d => d.Id == userId)                        
                        .FirstOrDefaultAsync();

                    var newDaily = new Dailydata
                    {
                        
                        Weight = last?.Weight ?? 0,
                        Dailykcalintake = last?.Kcalintake ?? 0,
                        Trainingdaytype = trainingDayType,
                        UserId = userId
                    };

                    await _context.Dailydatas.AddAsync(newDaily);
                }

                await _context.SaveChangesAsync();
            }



    }
}