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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.post)
            .HasForeignKey(c => c.PostId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

            modelBuilder.Entity<Comment>()
                        .HasMany(c => c.comments)
                        .WithOne(c => c.comment)
                        .HasForeignKey(c => c.ParentId);
        }
        public DbSet<Tips> Tips { get; set; }   

        public DbSet<Disease> Diseases { get; set; }

        public DbSet<Category> Categories { get; set; } 

        public DbSet<Treatment> Treatments { get; set; }    

        public DbSet<DiseaseTreatment> diseaseTreatments { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<PostReact> PostReactes { get; set; }
        public DbSet<CommentReact> CommentReactes { get; set; }
    
    }
}
