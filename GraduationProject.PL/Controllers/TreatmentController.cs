﻿using AutoMapper;
using BLL.IRepository;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : ControllerBase
    {
        private readonly IGenricRepository<Treatment> treatmentRepo;
        private readonly IMapper _mapper;

        public TreatmentController(IGenricRepository<Treatment> TreatmentRepo,
            IMapper mapper)
        {
            treatmentRepo = TreatmentRepo;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var treatments = await treatmentRepo.GetAll();
            var treatmentDto = _mapper.Map<IEnumerable<TreatmentDto>>(treatments).ToList();
            return Ok(treatments);
        }

        [HttpPost("AddTreatment")]
        public async Task<ActionResult> AddTreatment(TreatmentDto treatmentDto)
        {

            treatmentDto.Id = 0; 
            var treatment = _mapper.Map<Treatment>(treatmentDto);
            await treatmentRepo.Add(treatment);
            return Ok(new ApiRespones(200, "Treatment  added Successfully"));
        }
        [HttpDelete("DeleteTreatment")]
        public async Task<ActionResult> DeleteTreatment(int id)
        {
            var tip = await  treatmentRepo.GetById(id);
            if (tip is null)
                return NotFound(new ApiRespones(404));
            await treatmentRepo.Delete(tip);
            return Ok(new ApiRespones(200, "Treatment  Deleted Successfully"));
        }

        [HttpGet("GetTreatmentByid")]
        public async Task<ActionResult> GetTreatmentByid(int id)
        {
            var treatment = await treatmentRepo.GetById(id);
            if (treatment is null)
                return NotFound(new ApiRespones(404, "This Treatment not Found"));
            var treatmentDto = _mapper.Map<TreatmentDto>(treatment);
            return Ok(treatmentDto);
        }

        [HttpPut("EditTreatment")]

        public async Task<ActionResult> Edit(TreatmentDto treatmentDto)
        {
            
            var treatment = await treatmentRepo.GetById(treatmentDto.Id);
            if (treatment is null)
                return NotFound(new ApiRespones(404, $"No Treatment with {treatmentDto.Id}"));
            treatment.Name = treatmentDto.Name;
            treatment.Description = treatmentDto.Description; 
            await treatmentRepo.Update(treatment);
            return Ok(new ApiRespones(200, "Treatment  Updated Successfully"));
        }

    }
}