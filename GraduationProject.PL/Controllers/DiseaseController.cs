using AutoMapper;
using BLL.IRepository;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddDisease(AddDiseasesDto diseasesDto)
        {
            using var dateStream = new MemoryStream();
            await diseasesDto.Image.CopyToAsync(dateStream);
            var disease = new Disease()
            {
                Name = diseasesDto.Name,
                CategoryId = diseasesDto.CategoryId,
                symptoms = diseasesDto.symptoms,
                Reasons = diseasesDto.Reasons,
                Info = diseasesDto.Info,
                Image = dateStream.ToArray(),
                Treatments = diseasesDto.Treatments.Select(x => new DiseaseTreatment() { TreatmentId = x }).ToList(),

            };
            try
            {
                await _diseaseRepositry.Add(disease);
                return Ok(new ApiRespones(200, "Disease Added Successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRespones(404, ex.Message));
            }
            
            }

        [HttpGet("GetDiseaseById")]
        [Authorize]
        public async Task<ActionResult> GetDiseaseById(int id)
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
        [Authorize]
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
                       Treatments = d.Treatments.Select(d=> new TreatmentDto()
                       {
                           Id= d.Treatment.Id,
                           Name = d.Treatment.Name,
                           Description = d.Treatment.Description,
                           
                       }),
                       Image = d.Image,

                   }
                ).ToList(); 
            return Ok(resopone);
        }
        [HttpDelete("DeleteDisease")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDisease(int id)
        {
            var disease = await _diseaseRepositry.GetById(id);
            if (disease is null)
            {
                return NotFound(new ApiRespones(404, "Disease Not Found"));
            }
            try {
                await _diseaseRepositry.Delete(disease);
                return Ok(new ApiRespones(200, "Disease Deleted Successfully"));
            }
            catch (Exception ex)
            {
                 return BadRequest(new ApiRespones(404 , ex.Message));
            }
            }

        [HttpGet("SearchByName")]
        [Authorize]
        public async Task<ActionResult> SearchByName(string name)
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
