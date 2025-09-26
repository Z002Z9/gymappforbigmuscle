using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Dailydata;
using api.Dtos.Exercise;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDBContext _context;
        public ExerciseRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Exercise> CreateAsync(Exercise exerciseModel)
        {
            await _context.Exercises.AddAsync(exerciseModel);
            await _context.SaveChangesAsync();
            return exerciseModel;
        }

        public async Task<Exercise?> DeleteAsync(int id)
        {
            
            var exerciseModel = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == id);
            if (exerciseModel == null)
            {
                return null;
            }
            _context.Exercises.Remove(exerciseModel);
            await _context.SaveChangesAsync();
            return exerciseModel;
        }

        public async Task<List<Exercise>> GetAllAsync()
        {
            return await _context.Exercises.ToListAsync();
        }

        public async Task<Exercise?> GetByIdAsync(int id)
        {
            return await _context.Exercises.FindAsync(id);
        }

        public async Task<Exercise?> UpdateAsync(int id, UpdateExerciseRequestDto exerciseDto)
        {
            var existingExercise = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == id);
            if (existingExercise == null)
            {
                return null;
            }
            existingExercise.Name = exerciseDto.Name;
            existingExercise.Mainmuscle = exerciseDto.Mainmuscle;
            existingExercise.Youtubelink = exerciseDto.Youtubelink;
            existingExercise.Setnumber = exerciseDto.Setnumber;
            existingExercise.Repnumber = exerciseDto.Repnumber;
            existingExercise.Injuryblacklist = exerciseDto.Injuryblacklist;
            existingExercise.Bannedexercise = exerciseDto.Bannedexercise;

            await _context.SaveChangesAsync();
            return existingExercise;
        }
        
    }
}