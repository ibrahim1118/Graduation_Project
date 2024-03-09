using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class DiseaseTreatment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Disease))]
        public int DiseaseId { get; set; }

        [ForeignKey(nameof (Treatment))]
        public int TreatmentId { get; set; }
        public Disease Disease { get; set; }
        public Treatment Treatment { get; set; }    
    }
}
