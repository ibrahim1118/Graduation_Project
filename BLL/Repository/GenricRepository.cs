using BLL.IRepository;
using DAL.Data;
using DAL.Entity;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class GenricRepository<T> : IGenricRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenricRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(T item)
        {
          await _context.AddAsync(item);
          await _context.SaveChangesAsync();    
        }

        public async Task Delete(T item)
        {
            var type = typeof(T).Name; 
            if (type==nameof(Comment))
            {
                var commen = item as Comment;
                var rec = _context.CommentReactes.Where(c => c.CommentId == commen.Id); 
                _context.RemoveRange(rec);
            }
           _context.Set<T>().Remove(item);
           await _context.SaveChangesAsync();   
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var type = typeof(T).Name;
            if (type =="Disease")
              return await _context.Diseases.Include(d=>d.Category).Include(d=>d.Treatments).ThenInclude(d=>d.Treatment).ToListAsync() as IEnumerable<T>;
            if (type == nameof(Post))
                return await _context.Posts.OrderByDescending(p=>p.Id).Include(p=>p.AppUser).Include(p => p.postReacts).Include(d => d.Comments).ThenInclude(d => d.comment).Include(p=>p.AppUser) 
                    .ToListAsync() as IEnumerable<T>;
            if (type == nameof(Comment))
                return await _context.Comments.Include(c=>c.comments).Include(c=>c.AppUser)
                      .ToListAsync() as IEnumerable<T>;
            return await _context.Set<T>().ToListAsync(); 
        
        }

        public async Task<T?> GetById(int id)
        {
           var type =typeof(T).Name;
           if (type=="Disease")
           {
               return await _context.Diseases.Include(d => d.Category).Include(d=>d.Treatments).ThenInclude(d=>d.Treatment).FirstOrDefaultAsync(d => d.Id == id) as T; 
           }
           if (type == nameof(Post))
                return await  _context.Posts.Include(p=>p.postReacts).Include(p=>p.AppUser).Include(p=>p.Comments).FirstOrDefaultAsync(c=>c.Id==id) as T;
            if (type == nameof(Comment))
                return await _context.Comments.Include(c=>c.comments).Include(c=>c.AppUser).Include(p => p.comments).FirstOrDefaultAsync(c => c.Id == id) as T;
            return await _context.Set<T>().FindAsync(id); 
        }

        public async Task Update(T item)
        {
            _context.Set<T>().Update(item); 
            await _context.SaveChangesAsync();
        }
    }
}
