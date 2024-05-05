using DAL.AuthEntity;
using DAL.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.API.DTOS
{
    public class AddPostDto
    {
        public int Id { get; set; }

        public string Content { get; set; }
        public IFormFile? Image { get; set; }

       
      
        

    }
}
