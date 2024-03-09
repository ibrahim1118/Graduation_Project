using AutoMapper;
using DAL.AuthEntity;
using DAL.Entity;
using GraduationProject.API.DTOS;

namespace GraduationProject.API.Profiles
{
    public class MappingProFiles : Profile
    {
       public MappingProFiles() 
        { 
           CreateMap<Tips , TipsDto>().ReverseMap();
           CreateMap<Category , CategoryDto>().ReverseMap();
           CreateMap<AddDiseasesDto, Disease>().ReverseMap();
            CreateMap <AppUser , userDataDto>().ReverseMap();
        }
    }
}
