using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Dailydata;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class DailyDataRepository : IDailydataRepository
    {
        private readonly ApplicationDBContext _context;
        public DailyDataRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Dailydata> CreateAsync(Dailydata dailydataModel)
        {
            await _context.Dailydatas.AddAsync(dailydataModel);
            await _context.SaveChangesAsync();
            return dailydataModel;
        }

        public async Task<Dailydata?> DeleteAsync(int id)
        {
            
            var dailydataModel = await _context.Dailydatas.FirstOrDefaultAsync(x => x.Id == id);
            if (dailydataModel == null)
            {
                return null;
            }
            _context.Dailydatas.Remove(dailydataModel);
            await _context.SaveChangesAsync();
            return dailydataModel;
        }

        public async Task<List<Dailydata>> GetAllAsync()
        {
            return await _context.Dailydatas.ToListAsync();
        }

        public async Task<Dailydata?> GetByIdAsync(int id)
        {
            return await _context.Dailydatas.FindAsync(id);
        }

        public async Task<Dailydata?> UpdateAsync(int id, UpdateDailydataRequestDto dailydataDto)
        {
            var existingDailydata = await _context.Dailydatas.FirstOrDefaultAsync(x => x.Id == id);
            if (existingDailydata == null)
            {
                return null;
            }
            existingDailydata.Date = dailydataDto.Date;
            existingDailydata.Dailykcalintake = dailydataDto.Dailykcalintake;
            existingDailydata.Trainedtoday = dailydataDto.Trainedtoday;

            await _context.SaveChangesAsync();
            return existingDailydata;
        }
    }
}