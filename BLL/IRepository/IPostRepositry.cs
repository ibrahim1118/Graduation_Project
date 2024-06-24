using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface IPostRepositry : IGenricRepository<Post>
    {
        public Task AddReact(PostReact react);
        public Task DeleteReact(PostReact react);


        public IEnumerable<PostReact> GetAllReact();

        public PostReact GetReact(string userId, int postid);



    }
}
