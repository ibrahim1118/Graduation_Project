namespace GraduationProject.API.DTOS
{
    public class ForgertPasswordDto
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public string ConfirmCode { get; set; }
    }
}
