using System.ComponentModel.DataAnnotations;

namespace GraduationProject.API.DTOS
{
    public class GetDiseaseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Reasons { get; set; }
        [Required]
        public string Info { get; set; }
        [Required]
        public string symptoms { get; set; }

        public CategoryDto Category { get; set; }

        public IEnumerable<TreatmentDto> Treatments { get; set; }
        public byte[] Image { get; set; }
        public int CategoryId { get; set; }
    }
}
