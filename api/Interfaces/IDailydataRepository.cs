using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Models;

namespace api.Interfaces
{
    public interface IDailydataRepository
    {
        Task<List<Dailydata>> GetAllAsync();

        Task<Dailydata?> GetByIdAsync(int id);
        Task<Dailydata> CreateAsync(Dailydata dailydataModel);

        Task<Dailydata?> UpdateAsync(int id, UpdateDailydataRequestDto dailydataDto);

        Task<Dailydata?> DeleteAsync(int id);

    }
}