using Adminpanal.Hellper;
using AutoMapper;
using BLL.IRepository;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
       // [Authorize]
        public async Task<ActionResult> GetAll()
        {
            var Tips = await _tips.GetAll();
            var TipsDto = Tips.Select(t => new GetTipDto
            {
                id = t.id,
                Image = t.Image,
                Title = t.Title,
                description = t.Description,
                CreateDate = t.CreateDate

            }); 
           return Ok(Tips);
        }

        [HttpPost("AddTip")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> AddTip(AddTipsDto tipDto)
        {
            var ImageUrl = ""; 
            if (tipDto.Image is not null)
              ImageUrl= ImageSetting.UplodaImage(tipDto.Image, "Tips");
            var tip = new Tips
            {
                Title = tipDto.Title,
                Image = ImageUrl,
                Description = tipDto.description,
                CreateDate = DateTime.Now
            };
            try
            {
                await _tips.Add(tip); 
                return Ok(new ApiRespones(200, "tip added Successfully"));
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message)); 
            }
        }
        [HttpDelete("DeleteTip")]
       // [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTip(int id)
        {
            try {
                var tip = await _tips.GetById(id);
                if (tip is null)
                    return NotFound(new ApiRespones(404));
                if (tip.Image is not null)
                    ImageSetting.DeleteImage(tip.Image, "Tips");
                await _tips.Delete(tip);
                return Ok(new ApiRespones(200, "tip Deleted Successfully"));
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message)); 
            }
        }

        [HttpGet("GetTipById")]
        [Authorize]
        public async Task<ActionResult> GetTipByid(int id)
        {
            var tip = await _tips.GetById(id);
            if (tip is null)
                return NotFound(new ApiRespones(404 , "This tip not Found"));
            var tipDto = new GetTipDto()
            {
                id = tip.id,
                Image = tip.Image,
                description = tip.Description,
                CreateDate = tip.CreateDate,
                Title = tip.Title
            }; 
            return Ok(tipDto); 
        }

        [HttpPut("EditTip")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(AddTipsDto tipDto)
        {
            try {
                if (tipDto.id is null)
                    return BadRequest(new ApiRespones(400));
                var tip = await _tips.GetById(tipDto.id.Value);
                if (tip is null)
                    return NotFound(new ApiRespones(404, $"No Tips with {tipDto.id}"));
                if (tipDto.Image is not null)
                {
                    if (tip.Image != null)
                        ImageSetting.DeleteImage(tip.Image, "Tips");
                    tip.Image = ImageSetting.UplodaImage(tipDto.Image, "Tips");
                }
                tip.Description = tipDto.description;
                tip.Title = tipDto.Title;
                tip.CreateDate = DateTime.Now;
                await _tips.Update(tip);
                return Ok(new ApiRespones(200, "tip Updated Successfully"));
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400 ,ex.Message)); 
            }

        }

    }
}
