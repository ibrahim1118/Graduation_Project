namespace GraduationProject.API.DTOS
{
    public class GetCommentDto
    {
        public int Id { get; set; } 
        public string Content { get; set; }
        public string? Image { get; set; }

        public DateTime CreationDate { get; set; }
        public int likes { get; set; }
        public int DisLikes { get; set; }

        public HashSet<GetCommentDto> comments { get; set; } = new HashSet<GetCommentDto>();
        public string UserName { get; set; }
        public string? UserImage { get; set; }
        public string UserId { get; set; }
    }
}
