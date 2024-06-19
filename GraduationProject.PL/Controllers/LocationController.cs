using BLL.IRepository;
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
        private readonly ILocationRepository locationRepository;
        private readonly UserManager<AppUser> userManager;

        public LocationController(ILocationRepository locationRepository , UserManager<AppUser> userManager)
        {
            this.locationRepository = locationRepository;
            this.userManager = userManager;
        }

        [HttpPost("AddUserLocation")]
        [Authorize]
        public async Task<ActionResult> AddUserLocation(decimal Latitude, decimal Longitude)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest(new ApiRespones(400, "InValid Tokn"));
                }
                await locationRepository.Add(new UserLocation
                {
                    latitude = Latitude,
                    longitude = Longitude,
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
            var user =  locationRepository.GetNearestUsers(latitude, longitude);
            var res = user.Select(u => new UserLocationDto()
            {
                
                latitude = u.latitude,
                longitude = u.longitude,
                UserId = u.AppUser.Id,
                UserName = u.AppUser.FullName,
                Email = u.AppUser.Email
            }) ;
            return Ok(res);
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var res = await locationRepository.GetAll();
            return Ok(res); 
        }
    }
}
