using DAL.AuthEntity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public  class Post
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }
        public string? Image {  get; set; }

        public DateTime CreationDate { get; set; }
        public int likes { get; set; }
        public int DisLikes { get; set; }

        [ForeignKey(nameof(AppUser))]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public HashSet<PostReact> postReacts { get; set; }  
        public HashSet<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
