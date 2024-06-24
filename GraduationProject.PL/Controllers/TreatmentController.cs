using AutoMapper;
using BLL.IRepository;
using BLL.UnitOfwrok;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public TreatmentController(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
       
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<ActionResult> GetAll()
        {
            var treatments = await unitOfWork.Repostitry<Treatment>().GetAll();
            var treatmentDto = _mapper.Map<IEnumerable<TreatmentDto>>(treatments).ToList();
            return Ok(treatments);
        }

        [HttpPost("AddTreatment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddTreatment(TreatmentDto treatmentDto)
        {

            treatmentDto.Id = 0;
            var treatment = _mapper.Map<Treatment>(treatmentDto);
            try {
                await unitOfWork.Repostitry<Treatment>().Add(treatment);
                return Ok(new ApiRespones(200, "Treatment  added Successfully"));
             }
            catch (Exception ex)
            {
                return BadRequest(new ApiRespones(404, ex.Message));
            }
        }
        [HttpDelete("DeleteTreatment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTreatment(int id)
        {
            var tip = await unitOfWork.Repostitry<Treatment>().GetById(id);
            if (tip is null)
                return NotFound(new ApiRespones(404));
            try {
                await unitOfWork.Repostitry<Treatment>().Delete(tip);
                return Ok(new ApiRespones(200, "Treatment  Deleted Successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRespones(404, ex.Message));
            }
        }

        [HttpGet("GetTreatmentById")]
        [Authorize]
        public async Task<ActionResult> GetTreatmentByid(int id)
        {
            var treatment = await unitOfWork.Repostitry<Treatment>().GetById(id);
            if (treatment is null)
                return NotFound(new ApiRespones(404, "This Treatment not Found"));
            var treatmentDto = _mapper.Map<TreatmentDto>(treatment);
            return Ok(treatmentDto);
        }

        [HttpPut("EditTreatment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(TreatmentDto treatmentDto)
        {

            var treatment = await unitOfWork.Repostitry<Treatment>().GetById(treatmentDto.Id);
            if (treatment is null)
                return NotFound(new ApiRespones(404, $"No Treatment with {treatmentDto.Id}"));
            treatment.Name = treatmentDto.Name;
            treatment.Description = treatmentDto.Description;
            try {
                await unitOfWork.Repostitry<Treatment>().Update(treatment);
                return Ok(new ApiRespones(200, "Treatment  Updated Successfully"));
            }catch (Exception ex)
            {
                return BadRequest(new ApiRespones(404 ,ex.Message));
            }
             
            }

    }
}
