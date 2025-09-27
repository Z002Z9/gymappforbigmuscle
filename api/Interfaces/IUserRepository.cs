using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Dailydata;
using api.Models;
using gymappforbigmuscle.Dtos.User;

namespace gymappforbigmuscle.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User userModel);

        Task<User?> UpdateAsync(int id, UpdateUserRequestDto userDto);

        Task<User?> DeleteAsync(int id);

        Task<User?> GetByEmailAsync(string email);

    }
}