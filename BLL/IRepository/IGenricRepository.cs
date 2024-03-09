using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface IGenricRepository<T> where T : class
    {
        public  Task<IEnumerable<T>> GetAll();

        public  Task<T?> GetById(int id);

        public Task Add(T item);

        public Task Update(T item);

        public Task Delete(T item);

    }
}
