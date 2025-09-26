using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<Dailydata> Dailydatas { get; set; } 
        
        public DbSet<Trainingprogram> Trainingprograms { get; set; } 
            
        
    }
}