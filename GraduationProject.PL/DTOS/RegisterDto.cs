using System.ComponentModel.DataAnnotations;
namespace GraduationProject.API.DTOS
{
    public class RegisterDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
