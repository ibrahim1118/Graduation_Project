using BLL.IRepository;
using BLL.Repository;
using DAL.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.UnitOfwrok
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        private Hashtable repostitry;
 
        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
            repostitry = new Hashtable();
        }

        public ICommentRepositry CommentRepositry()
        {
            return new CommentRepositry(context) as ICommentRepositry;
        }

        public IDiseaseRepositry DiseaseRepositry()
        {
           return  new DiseaseRepositry(context) as IDiseaseRepositry;
        }

        public async ValueTask DisposeAsync()
        {
             await context.DisposeAsync();
        }

        public async Task<int> IsComplet()
        {
            return await context.SaveChangesAsync(); 
        }

        public ILocationRepository locationRepository()
        {
           return new LocationRepository(context) as ILocationRepository;
        }

        public IPostRepositry PostRepositry()
        {
            return new PostRepostiry(context) as IPostRepositry;
        }

        public IGenricRepository<T> Repostitry<T>() where T : class
        {
            var type = typeof(T).Name;
            if (!repostitry.ContainsKey(type))
            {
                repostitry.Add(type, new GenricRepository<T>(context));
            }
            return repostitry[type] as IGenricRepository<T>;
        }

        public IReviewRepository ReviewRepository()
        {
            return new  ReviewsRepositry(context) as IReviewRepository;
        }
    }
}
