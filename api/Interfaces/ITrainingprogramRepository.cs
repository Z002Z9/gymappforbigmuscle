using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Dtos.Trainingprogram;
using api.Models; 

namespace api.Interfaces
{
    public interface ITrainingprogramRepository
    {
        Task<List<Trainingprogram>> GetAllAsync();

        Task<Trainingprogram?> GetByIdAsync(int id);
        Task<Trainingprogram> CreateAsync(Trainingprogram trainingprogramModel);

        Task<Trainingprogram?> UpdateAsync(int id, UpdateTrainingprogramRequestDto trainingprogramDto);

        Task<Trainingprogram?> DeleteAsync(int id);
    }
}