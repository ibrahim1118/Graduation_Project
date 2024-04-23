using DAL.AuthEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public  class Comment
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }
        public string? Image { get; set; }

        public DateTime CreationDate { get; set; }
        public int likes { get; set; }
        public int DisLikes { get; set; }
        [ForeignKey(nameof(AppUser))]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public IEnumerable<CommentReact> Reactes { get; set; } = new List<CommentReact>(); 
        [ForeignKey(nameof(comment))]
        public int? ParentId { get; set; }
        public Comment? comment { get; set; }
        public HashSet<Comment> comments { get; set; } = new HashSet<Comment>();

        [ForeignKey(nameof(post))]
        public int PostId { get; set; }
        public Post post { get; set; }

    }
}
