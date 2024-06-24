using BLL.IRepository;
using DAL.Data;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class CommentRepositry : GenricRepository<Comment> , ICommentRepositry
    {
        private readonly AppDbContext context;

        public CommentRepositry(AppDbContext context): base(context) 
        {
            this.context = context;
        }

        
        public async Task AddReplie(Comment comment)
        {
            var pernt = context.Comments.Find(comment.ParentId); 
            if (pernt == null) {
                return; 
            }
            comment.PostId = pernt.PostId;
            if (pernt.ParentId is not null) 
            { 
              pernt = context.Comments.Find(pernt.ParentId);
            }
            comment.ParentId = pernt.Id; 
            pernt.comments.Add(comment);
            context.SaveChanges();
        }
        public async Task AddReact(CommentReact react)
        {
            var rec = await context.CommentReactes.FirstOrDefaultAsync(r => r.CommentId == react.CommentId && r.userId == react.userId);
            var comment = context.Comments.Find(react.CommentId);
            if (rec is null && (react.like || react.disLike))
            {
                if (react.disLike)
                {
                    comment.DisLikes++;
                }
                else
                    comment.likes++;
                context.CommentReactes.Add(react);
            }
            else
            {
                if (react.like)
                {
                    if (rec.like)
                    {
                        rec.like = false;
                        comment.likes--;
                    }
                    else
                    {
                        rec.like = true;
                        comment.likes++;
                        if (rec.disLike)
                        {
                            rec.disLike = false;
                            comment.DisLikes--;
                        }
                    }
                }
                else if (react.disLike)
                {
                    if (rec.disLike)
                    {
                        rec.disLike = false;
                        comment.DisLikes--;
                    }
                    else
                    {
                        rec.disLike = true;
                        comment.DisLikes++;
                        if (rec.like)
                        {
                            rec.like = false;
                            comment.likes--;
                        }
                    }

                }
                context.CommentReactes.Update(rec);
                if (!rec.like && !rec.disLike)
                    context.CommentReactes.Remove(rec);
            }
            await context.SaveChangesAsync();

        }

        public async Task DeleteReact(CommentReact react)
        {
            context.CommentReactes.Remove(react);
           await context.SaveChangesAsync(); 
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(int Postid)
        {
            return await context.Comments.Where(c => c.PostId == Postid).Include(c => c.AppUser).ToArrayAsync() ; 
        }

        public IEnumerable<CommentReact> GetAllReact()
        {
              return context.CommentReactes.ToList();
        }

        public CommentReact GetReact(int CommentId, string UserId)
        {
            return context.CommentReactes.FirstOrDefault(r => r.CommentId == CommentId && r.userId == UserId); 
        }
    }
}
