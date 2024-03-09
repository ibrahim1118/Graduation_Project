using BLL.IRepository;
using DAL.Data;
using DAL.Entity;
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
           _context.Set<T>().Remove(item);
           await _context.SaveChangesAsync();   
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var type = typeof(T).Name;
            if (type =="Disease")
              return await _context.Diseases.Include(d=>d.Category).ToListAsync() as IEnumerable<T>;

            return await _context.Set<T>().ToListAsync(); 
        
        }

        public async Task<T?> GetById(int id)
        {
           var type =typeof(T).Name;
           if (type=="Disease")
           {
               return await _context.Diseases.Include(d => d.Category).FirstOrDefaultAsync(d => d.Id == id) as T; 
           }
            return await _context.Set<T>().FindAsync(id); 
        }

        public async Task Update(T item)
        {
            _context.Set<T>().Update(item); 
            await _context.SaveChangesAsync();
        }
    }
}
