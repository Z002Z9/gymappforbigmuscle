using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Dailydata;
using api.Dtos.Trainingprogram;
using api.Interfaces;
using api.Models;
using gymappforbigmuscle.Dtos.User;
using gymappforbigmuscle.Interfaces;
using Microsoft.EntityFrameworkCore; 

namespace gymappforbigmuscle.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;
        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(User userModel)
        {
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            return userModel;
        }

        public async Task<User?> DeleteAsync(int id)
        {
            
            var userModel = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userModel == null)
            {
                return null;
            }
            _context.Users.Remove(userModel);
            await _context.SaveChangesAsync();
            return userModel;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> UpdateAsync(int id, UpdateUserRequestDto userDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (existingUser == null)
            {
                return null;
            }
            existingUser.Name = userDto.Name;
            existingUser.Email = userDto.Email;
            existingUser.Password = userDto.Password;
            existingUser.Age = userDto.Age;
            existingUser.Height = userDto.Height;
            existingUser.Injury = userDto.Injury;
            existingUser.Allergys = userDto.Allergys;
            existingUser.Kcalintake = userDto.Kcalintake;
            existingUser.Trainingtype = userDto.Trainingtype;
            existingUser.Trainingsperweek = userDto.Trainingsperweek;



            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}