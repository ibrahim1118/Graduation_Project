using BLL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.UnitOfwrok
{
    public interface IUnitOfWork : IAsyncDisposable
    {

        public IGenricRepository<T> Repostitry<T>() where T : class;

        public ICommentRepositry CommentRepositry(); 
        public IDiseaseRepositry DiseaseRepositry(); 
        public ILocationRepository locationRepository();
        
        public IPostRepositry PostRepositry();
        public IReviewRepository ReviewRepository();
     
       
        public Task<int> IsComplet();
    }
}
