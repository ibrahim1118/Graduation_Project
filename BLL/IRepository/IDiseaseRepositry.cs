using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface IDiseaseRepositry : IGenricRepository<Disease>
    {

        public Task<IEnumerable<Disease>> Serach(string name);
    }
}
