using AutoMapper;
using BLL.IRepository;
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
    [Authorize]
    public class TipsController : ControllerBase
    {
        private readonly IGenricRepository<Tips> _tips;
        private readonly IMapper _mapper;

        public TipsController(IGenricRepository<Tips> tips,IMapper mapper)
        {
            _tips = tips;
           _mapper = mapper;
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<ActionResult> GetAll()
        {
            var Tips = await _tips.GetAll();  
            var TipsDto = _mapper.Map<IEnumerable<TipsDto>>(Tips).ToList();
           return Ok(Tips);
        }

        [HttpPost("AddTip")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> AddTip(TipsDto tipDto)
        {
            tipDto.CreateDate = DateTime.Now;
            var tip = _mapper.Map<Tips>(tipDto);
            await _tips.Add(tip);
            return Ok(new ApiRespones(200 , "tip added Successfully"));
        }
        [HttpDelete("DeleteTip")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTip(int id)
        {
            var tip = await _tips.GetById(id);
            if (tip is null)
                return NotFound(new ApiRespones(404));
           await _tips.Delete(tip);
            return Ok(new ApiRespones(200 , "tip Deleted Successfully")); 
        }

        [HttpGet("GetTipById")]
        [Authorize]
        public async Task<ActionResult> GetTipByid(int id)
        {
            var tip = await _tips.GetById(id);
            if (tip is null)
                return NotFound(new ApiRespones(404 , "This tip not Found")); 
            var tipDto= _mapper.Map<TipsDto>(tip);
            return Ok(tipDto); 
        }

        [HttpPut("EditTip")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(TipsDto tipDto)
        {
            if (tipDto.id is null)
                return BadRequest(new ApiRespones(400));
            var tip = await _tips.GetById(tipDto.id.Value);
            if (tip is null)
                return NotFound(new ApiRespones(404 , $"No Tips with {tipDto.id}"));
            tip.tips = tipDto.tips;
            tip.CreateDate = DateTime.Now;
           await _tips.Update(tip);
            return Ok(new ApiRespones(200 , "tip Updated Successfully")); 
        }

    }
}
