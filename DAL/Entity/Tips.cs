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

        public string Title {  get; set; }
        public string? Image {  get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
