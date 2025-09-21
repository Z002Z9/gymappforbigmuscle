using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Dtos.Exercise;
using api.Models;

namespace api.Interfaces
{
    public interface IExerciseRepository
    {
        Task<List<Exercise>> GetAllAsync();

        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> CreateAsync(Exercise exerciseModel);

        Task<Exercise?> UpdateAsync(int id, UpdateExerciseRequestDto exerciseDto);
        //true a default value az updatenal es otletem sincsen miert szoval egyenlore jo lesz

        Task<Exercise?> DeleteAsync(int id);
    }
}