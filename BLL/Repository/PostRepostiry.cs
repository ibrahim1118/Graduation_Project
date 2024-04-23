using BLL.IRepository;
using DAL.Data;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class PostRepostiry : GenricRepository<Post>, IPostRepositry
    {
        private readonly AppDbContext context;

        public PostRepostiry(AppDbContext context): base(context)         {
                
               this.context = context;
        }
        public async Task AddReact(PostReact react)
        {
            var rec = await context.PostReactes.FirstOrDefaultAsync(r => r.PostId == react.PostId && r.userId == react.userId);
            var post = context.Posts.Find(react.PostId);
            if (rec is null&&(react.like||react.disLike))
            {
                if (react.disLike)
                {
                    post.DisLikes++;
                }
                else
                    post.likes++; 
                context.PostReactes.Add(react);
            }
            else
            {
                if (react.like)
                {
                    if (rec.like)
                    {
                        rec.like = false;
                        post.likes--; 
                    }
                    else
                    {
                        rec.like = true;
                        post.likes++; 
                        if (rec.disLike)
                        {
                            rec.disLike = false;
                            post.DisLikes--; 
                        }
                    }
                }
                else if (react.disLike)
                {
                    if (rec.disLike)
                    {
                        rec.disLike = false;
                        post.DisLikes--; 
                    }
                    else
                    {
                        rec.disLike = true;
                        post.DisLikes++; 
                        if (rec.like)
                        {
                            rec.like = false;
                            post.likes--; 
                        }
                    }

                }
                context.PostReactes.Update(rec); 
                if (!rec.like&&!rec.disLike)
                    context.PostReactes.Remove(rec);
            }
            await context.SaveChangesAsync();

        }

        public async Task DeleteReact(PostReact react)
        {
            context.PostReactes.Remove(react); 
            await context.SaveChangesAsync();
        }
    }
}
