using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class Tips
    {
        public int id {  get; set; }

        public string tips { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
