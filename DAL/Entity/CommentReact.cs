using DAL.AuthEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class CommentReact
    {
        public int Id { get; set; }

        //[ForeignKey(nameof(PostId))]
        public Comment comment { get; set; }
        public int CommentId { get; set; }

        [ForeignKey(nameof(userId))]
        public AppUser appUser { get; set; }

        public string userId { get; set; }

        public bool like { get; set; }

        public bool disLike { get; set; }
    }
}
