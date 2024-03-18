using System.ComponentModel.DataAnnotations;

namespace GraduationProject.API.DTOS
{
    public class LoginDTO
    {

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }    
    }
}
