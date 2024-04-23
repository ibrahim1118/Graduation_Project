using DAL.AuthEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class PostReact
    {
       public int id { get; set; }

        //[ForeignKey(nameof(PostId))]
        public Post post { get; set; }
        public int PostId { get; set; }

       [ForeignKey(nameof(userId))]
       public AppUser appUser { get; set; }

        public string userId { get; set; }

        public bool like { get; set; }

        public bool disLike { get; set; }
       
    }
}
