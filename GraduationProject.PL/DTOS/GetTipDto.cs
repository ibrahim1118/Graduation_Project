namespace GraduationProject.API.DTOS
{
    public class GetTipDto
    {
        public int? id { get; set; }
        public string Title { get; set; }
        public string description { get; set; }
        public string? Image { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
