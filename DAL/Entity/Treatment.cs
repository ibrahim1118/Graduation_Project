using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class Treatment
    {
        public int Id { get; set; } 

        public int Name { get; set; }

        public string Description { get; set; }

        public byte[] image {  get; set; }
    }
}
