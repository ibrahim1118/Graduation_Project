using System.ComponentModel.DataAnnotations;

namespace GraduationProject.API.DTOS
{
    public class AddDiseasesDto
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]

        public string Reasons { get; set; }
        [Required]
        public string Info { get; set; }
        [Required]
        public string symptoms { get; set; }

        public IFormFile Image { get; set; }
        public int CategoryId { get; set; }

        public List<int> Treatments { get; set; } = new List<int>();
    }
}
