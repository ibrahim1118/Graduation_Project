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
    public class ReviewsRepositry : GenricRepository<UserReview>, IReviewRepository
    {
        private readonly AppDbContext context;

        public ReviewsRepositry(AppDbContext context): base(context) 
        {
            this.context = context;
        }
        public async Task<IEnumerable<UserReview>> GetReviewsbyType(string type)
        {
            return await context.UserReviews.Where(r=>r.type == type).ToListAsync();
        }
    }
}
