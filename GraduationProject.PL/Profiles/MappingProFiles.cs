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
            CreateMap<Post, GetPostDto>()
                .ForMember(p => p.Id, p => p.MapFrom(x => x.Id)).
                 ForMember(p => p.Content, p => p.MapFrom(x => x.Content)).
                 ForMember(p => p.Image, p => p.MapFrom(x => x.Image)).ReverseMap(); 
            CreateMap<Comment , GetCommentDto>().ReverseMap();
        }
    }
}
