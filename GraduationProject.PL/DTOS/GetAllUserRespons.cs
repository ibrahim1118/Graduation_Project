namespace GraduationProject.API.DTOS
{
    public class GetAllUserRespons
    {
        public int UsersNumber { get; set; }

       public IEnumerable<userDataDto> Users { get; set;  }
    }
}
