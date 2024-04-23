namespace GraduationProject.API.DTOS
{
    public class AddCommentDto
    {
        public int Id { get; set; }  

        public string Content { get; set; }
        public IFormFile? Image { get; set; }

        public int? ParentId { get; set; }
        public int PostId { get; set; } = 0; 
    }
}
