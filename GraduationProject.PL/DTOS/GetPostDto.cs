namespace GraduationProject.API.DTOS
{
    public class GetPostDto
    { 
        public int Id { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }

        public DateTime CreationDate { get; set; }
        public int likes { get; set; }
        public int DisLikes { get; set; }
        public HashSet<GetCommentDto>? Comments { get; set; }  = new HashSet<GetCommentDto>();
       
        public string UserId { get; set; }
    }
}
