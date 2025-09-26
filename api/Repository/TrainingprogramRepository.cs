using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Dailydata;
using api.Dtos.Trainingprogram;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class TrainingprogramRepository : ITrainingprogramRepository
    {
        private readonly ApplicationDBContext _context;
        public TrainingprogramRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Trainingprogram> CreateAsync(Trainingprogram trainingprogramModel)
        {
            await _context.Trainingprograms.AddAsync(trainingprogramModel);
            await _context.SaveChangesAsync();
            return trainingprogramModel;
        }

        public async Task<Trainingprogram?> DeleteAsync(int id)
        {
            
            var trainingprogramModel = await _context.Trainingprograms.FirstOrDefaultAsync(x => x.Id == id);
            if (trainingprogramModel == null)
            {
                return null;
            }
            _context.Trainingprograms.Remove(trainingprogramModel);
            await _context.SaveChangesAsync();
            return trainingprogramModel;
        }

        public async Task<List<Trainingprogram>> GetAllAsync()
        {
            return await _context.Trainingprograms.ToListAsync();
        }

        public async Task<Trainingprogram?> GetByIdAsync(int id)
        {
            return await _context.Trainingprograms.FindAsync(id);
        }

        public async Task<Trainingprogram?> UpdateAsync(int id, UpdateTrainingprogramRequestDto trainingprogramDto)
        {
            var existingTrainingprogram = await _context.Trainingprograms.FirstOrDefaultAsync(x => x.Id == id);
            if (existingTrainingprogram == null)
            {
                return null;
            }
            existingTrainingprogram.Name = trainingprogramDto.Name;
            existingTrainingprogram.Plan = trainingprogramDto.Plan;
            existingTrainingprogram.Daysperweek = trainingprogramDto.Daysperweek;
            existingTrainingprogram.Kcalburned = trainingprogramDto.Kcalburned;

            await _context.SaveChangesAsync();
            return existingTrainingprogram;
        }
    }
}