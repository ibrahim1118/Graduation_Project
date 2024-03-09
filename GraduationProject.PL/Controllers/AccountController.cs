using AutoMapper;
using Azure.Core;
using DAL.AuthEntity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using GraduationProject.API.Helpear;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Service.ImpService;
using Service.IService;
using System.Security.Claims;
namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager ,
            SignInManager<AppUser> signInManager, 
             ITokenService tokenService , 
             IMapper mapper
             )
        {
            _userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }


        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser() 
        {

            var use = _userManager.Users.ToList();//.Skip
                //(1).Take(3).ToList();
            
            var user = mapper.Map<IEnumerable<userDataDto>>(use);
            return Ok(user);
          
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (ChackEamil(model.Email).Result.Value)
                return BadRequest(new ApiRespones(400)
                {
                    Message  = "This email already exists"
                });
            var user = new AppUser()
            {
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                FullName = model.FullName,
            };
            var res = await _userManager.CreateAsync(user, model.Password);
            if (!res.Succeeded)
                return BadRequest(new ApiRespones(400));
            return Ok(new UserDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = await tokenService.CreatTokeAysnc(user, _userManager)
            });
        }

        [HttpPost("longin")]
        public async Task<ActionResult<UserDto>> longin(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new ApiRespones(401));
            }
            var res = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!res.Succeeded)
                return Unauthorized(new ApiRespones(401));
            return Ok(new UserDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = await tokenService.CreatTokeAysnc(user, _userManager)
            });

        }
        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgertPassword(string Email)
        {

            var user = await _userManager.FindByEmailAsync(Email); 
            if (user is null)
            {
                return NotFound(new ApiRespones(404)); 
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var resetPasswordLink = Url.Action("ResetPassword", "Account", new { Email = Email, Token = token }, Request.Scheme);

            var email = new Email()
            {
                Subject = "Reset Your Password",
                To = Email,
                Body = "ForntEndURL"
            };
            EmailSetting.SendEmail(email);
            return Ok(new ForgertPasswordDto
            {
                Email = Email,
                Token = token
            }); ; 

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            

                var user = await _userManager.FindByEmailAsync(model.Email);
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                    return Ok(new ApiRespones(200 , "Password Changed Success"));

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

             return BadRequest(ModelState);
            
        }
        [HttpPost("CheangePassword")]
        [Authorize]
        public async Task<ActionResult> ChengePassword(ChangePasswordDto dto)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return Unauthorized(new ApiRespones(401));

            var res = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (res.Succeeded)
                return Ok(new ApiRespones(200 , "password Changed Succefule"));
            foreach ( var  Erorr in res.Errors)
            {
                ModelState.AddModelError("", Erorr.Description); 
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string Email)
        {
            var user =await _userManager.FindByEmailAsync(Email);
            var res = await _userManager.DeleteAsync(user);
            if (res.Succeeded)
                return Ok(new ApiRespones(200, "User Delete Succe"));
            foreach (var Eror in res.Errors)
                ModelState.AddModelError("", Eror.Description); 
             return BadRequest(ModelState);
        }
       
        [HttpGet("EmailExited")]
        public async Task<ActionResult<bool>> ChackEamil(string Email)
        {
            return await _userManager.FindByEmailAsync(Email) is not null;
        }
    }
}
