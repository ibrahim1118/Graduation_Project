namespace GraduationProject.API.DTOS
{
    public class AddTipsDto
    {
        public int? id { get; set; }

        public string Title {  get; set; }
        public string description { get; set; }
        public IFormFile Image { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
