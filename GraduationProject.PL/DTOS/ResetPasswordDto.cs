using System.ComponentModel.DataAnnotations;

namespace GraduationProject.API.DTOS
{
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public string NewConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
