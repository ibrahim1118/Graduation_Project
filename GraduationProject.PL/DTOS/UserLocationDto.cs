namespace GraduationProject.API.DTOS
{
    public class UserLocationDto
    {
        public decimal latitude {  get; set; }
        public decimal longitude { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

    }
}
