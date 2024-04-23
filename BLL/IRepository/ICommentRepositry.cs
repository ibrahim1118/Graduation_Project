using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface ICommentRepositry : IGenricRepository<Comment>
    {
        public Task AddReplie(Comment comment);

        public Task AddReact(CommentReact react);
        public Task DeleteReact(CommentReact react);
    }
}
