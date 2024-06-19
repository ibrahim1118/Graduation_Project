namespace GraduationProject.API.DTOS
{
    public class UserDto{ 
           public string FullName { get; set; }
           
           public string UserId { get; set; }
           public string Email { get; set; }
           public string? Image {  get; set; }
           public bool ? IsAdmin { get; set; }
           public string Token { get; set; }
        
    }
}
