using DAL.AuthEntity;
using DAL.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) 
        {
            
        }
        public DbSet<Tips> Tips { get; set; }   

        public DbSet<Disease> Diseases { get; set; }

        public DbSet<Category> Categories { get; set; } 

        public DbSet<Treatment> Treatments { get; set; }    

        public DbSet<DiseaseTreatment> diseaseTreatments { get; set; }

    }
}
