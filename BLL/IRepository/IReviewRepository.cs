using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface IReviewRepository : IGenricRepository<UserReview>
    {
        public Task<IEnumerable<UserReview>> GetReviewsbyType(string type); 
    }
}
