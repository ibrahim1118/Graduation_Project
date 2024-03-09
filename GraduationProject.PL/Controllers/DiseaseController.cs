using AutoMapper;
using BLL.IRepository;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiseaseController : ControllerBase
    {
        private readonly IDiseaseRepositry _diseaseRepositry;
        private readonly IMapper mapper;

        public DiseaseController(IDiseaseRepositry diseaseRepositry , 
            IMapper mapper)
        {
            _diseaseRepositry = diseaseRepositry;
            this.mapper = mapper;
        }

        [HttpPost("AddDisease")]
        public async Task<ActionResult> AddDisease(AddDiseasesDto diseasesDto)
        {
            using var dateStream = new  MemoryStream();
            await diseasesDto.Image.CopyToAsync(dateStream);
            var disease = new Disease()
            {
                Name = diseasesDto.Name,
                CategoryId = diseasesDto.CategoryId,
                symptoms = diseasesDto.symptoms,
                Reasons = diseasesDto.Reasons,
                Info = diseasesDto.Info,    
                Image = dateStream.ToArray()
            };
            await  _diseaseRepositry.Add(disease); 
            return Ok(new ApiRespones(200 , "Disease Added Succefule"));
        }

        [HttpGet("GetDiseaseByid")]
        public async Task<ActionResult> GetDiseasebyid(int id)
        {
            var disease = await _diseaseRepositry.GetById(id);
            if (disease == null)
            {
                return NotFound(new ApiRespones(404  , "Disease Not Found"));  
            }
            var resopone = new GetDiseaseDto()
            {
                Id = disease.Id,
                Name = disease.Name,
                Reasons = disease.Reasons,
                symptoms = disease.symptoms,
                Category = new CategoryDto()
                {
                    Id = disease.Category.Id,
                    Name = disease.Category.Name
                },
                Image = disease.Image,

            };
            return Ok(resopone); 
        }
        [HttpGet("GetAllDisease")]
        public async Task<ActionResult> GetAllDisease()
        {
            var disease = await _diseaseRepositry.GetAll();
            if (disease == null)
            {
                return NotFound();
            }
            var resopone = disease.Select(
                   d => new GetDiseaseDto()
                   {
                       Id = d.Id,
                       Name = d.Name,
                       Reasons = d.Reasons,
                       Info = d.Info,
                       symptoms = d.symptoms,
                       Category = new CategoryDto()
                       {
                           Id = d.Category.Id,
                           Name = d.Category.Name
                       },
                       Image = d.Image,

                   }
                ).ToList(); 
            return Ok(resopone);
        }
        [HttpDelete("DeleteDisease")]
        public async Task<ActionResult> DeleteDiseas(int id)
        {
            var disease = await _diseaseRepositry.GetById(id);
            if (disease is null) 
            {
                return NotFound(new ApiRespones(404 ,"Disease Not Found")); 
            }
            await _diseaseRepositry.Delete(disease);
            return Ok(new ApiRespones(200 , "Disease Deleted"));
        }

        [HttpGet("SearchByName")]
        public async Task<ActionResult> SearchbyName(string name)
        {
            var disease = await _diseaseRepositry.Serach(name);
            if (disease is null)
                return NotFound(new ApiRespones(404, "No Disease With this Name"));
            var resopone = disease.Select(
                  d => new GetDiseaseDto()
                  {
                      Id = d.Id,
                      Name = d.Name,
                      Reasons = d.Reasons,
                      Info = d.Info,
                      symptoms = d.symptoms,
                      Category = new CategoryDto()
                      {
                          Id = d.Category.Id,
                          Name = d.Category.Name
                      },
                      Image = d.Image,

                  }
               ).ToList();
            return Ok(resopone);
        }
    }
    
}
