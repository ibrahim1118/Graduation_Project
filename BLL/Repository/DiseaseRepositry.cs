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
    public class DiseaseRepositry : GenricRepository<Disease>, IDiseaseRepositry
    {
        private readonly AppDbContext _context;

        public DiseaseRepositry(AppDbContext context): base(context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<Disease>> Serach(string name)
        {
            var  dises = await _context.Diseases.Include(d => d.Category)
                .Where(d => d.Name.Contains(name.ToLower())).ToListAsync();
            
            return dises; 
        }
    }
}
