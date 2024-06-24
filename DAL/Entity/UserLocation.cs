using DAL.AuthEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class UserLocation
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }

        public string?  Token { get; set; }
        public AppUser? AppUser { get; set; }




    }
}
