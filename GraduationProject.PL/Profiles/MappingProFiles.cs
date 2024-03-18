using AutoMapper;
using DAL.AuthEntity;
using DAL.Entity;
using GraduationProject.API.DTOS;
using Microsoft.AspNetCore.Identity;

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
            CreateMap<IdentityRole, RoleDto>().ReverseMap(); 
            CreateMap<Treatment , TreatmentDto>().ReverseMap(); 
        }
    }
}
