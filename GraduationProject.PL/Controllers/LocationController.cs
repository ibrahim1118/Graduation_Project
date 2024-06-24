using BLL.IRepository;
using BLL.UnitOfwrok;
using DAL.AuthEntity;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public LocationController(IUnitOfWork unitOfWork , UserManager<AppUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        [HttpPost("AddUserLocation")]
        [Authorize]
        public async Task<ActionResult> AddUserLocation(LocationDto locationDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest(new ApiRespones(400, "InValid Tokn"));
                }
                await unitOfWork.locationRepository().Add(new UserLocation
                {
                    latitude = locationDto.Latitude,
                    longitude = locationDto.Longitude,
                    AppUserId = userId
                });
                return Ok(new ApiRespones(200 , "Location Added"));
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message)); 
            }
        }
        [HttpGet("GetNearestUsers")]
        public async Task<ActionResult> GetNearestUsers (decimal latitude,  decimal longitude)
        {
            var user =  unitOfWork.locationRepository().GetNearestUsers(latitude, longitude);
            HashSet<UserLocationDto> res = user.Select(u => new UserLocationDto()
            {
                Token = u.Token,
                UserId = u.AppUserId,
            }).ToHashSet<UserLocationDto>() ;
            return Ok(res);
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var res = await unitOfWork.locationRepository().GetAll();
            return Ok(res); 
        }
        [HttpPost("AddUserToken")] 
        public async Task<ActionResult> AddUserToken(string userId, string Token)
        {
            try {
                await unitOfWork.locationRepository().AddUserToken(userId, Token);
                return Ok(new ApiRespones(200 , "Token Added"));
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message)); 
            }

        }

    }
}
