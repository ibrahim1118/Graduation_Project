using Adminpanal.Hellper;
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
using static System.Net.Mime.MediaTypeNames;
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

        [HttpGet("Profile")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUser()
        {
          
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return Unauthorized(new ApiRespones(401));
            var userdto = new UserDto
            {
                UserId = user.Id,
                Image = user.Image,
                FullName = user.FullName,
                Email = user.Email
            }; 
            return Ok(userdto);
        }
        [HttpPut("ChangeName")]
        [Authorize]
        public async Task<ActionResult> ChangeName(string NewName)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return Unauthorized(new ApiRespones(401));
            user.FullName = NewName; 
            await _userManager.UpdateAsync(user);
            var userDot = mapper.Map<userDataDto>(user);
            return Ok(userDot); 
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUser(int pageNumber, int pageSize)
        {

            var use = await _userManager.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var user = mapper.Map<IEnumerable<userDataDto>>(use);
            var respone = new GetAllUserRespons()
            {
                UsersNumber = _userManager.Users.Count(),
                Users = user
            }; 
            return Ok(respone);
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEamil(model.Email).Result.Value)
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
            {
                foreach (var erorr  in res.Errors)
                {
                    ModelState.AddModelError("", erorr.Description); 
                }
                return BadRequest(ModelState); 
            }
            return Ok(new UserDto()
            {
                UserId = user.Id, 
                FullName = user.FullName,
                Email = user.Email,
                Token = await tokenService.CreatTokeAysnc(user, _userManager)
            }) ;
        }

        [HttpPost("logIn")]
        public async Task<ActionResult<UserDto>> lonIn(LoginDTO model)
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
                UserId = user.Id,
                Image = user.Image,
                IsAdmin = await _userManager.IsInRoleAsync(user , "Admin"),
                Token = await tokenService.CreatTokeAysnc(user, _userManager)
            }); ;

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
            Random r = new Random();
            var ConfirmationCode = r.Next(100000, 999999).ToString();

            var email = new Email()
            {
                Subject = "Reset Your Password",
                To = Email,
                Body = $"Confirmation Code : {ConfirmationCode}"
            };
            EmailSetting.SendEmail(email);
            return Ok(new ForgertPasswordDto
            {
                Email = Email,
                Token = token,
                ConfirmCode = ConfirmationCode
            }); ;

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            

                var user = await _userManager.FindByEmailAsync(model.Email);
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                    return Ok(new ApiRespones(200 , "Password Changed Successfully"));

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

             return BadRequest(ModelState);
            
        }
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return Unauthorized(new ApiRespones(401));

            var res = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (res.Succeeded)
                return Ok(new ApiRespones(200 , "password Changed Successfully"));
            foreach ( var  Erorr in res.Errors)
            {
                ModelState.AddModelError("", Erorr.Description); 
            }
            return BadRequest(ModelState);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser(string Email)
        {
            var user =await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return NotFound(new ApiRespones(404)); 
            var res = await _userManager.DeleteAsync(user);
            if (res.Succeeded)
                return Ok(new ApiRespones(200, "User Deleted Successfully"));
            foreach (var Eror in res.Errors)
                ModelState.AddModelError("", Eror.Description); 
             return BadRequest(ModelState);
        }
        [HttpPost("AddUserImage")]
        [Authorize]
        public async Task<ActionResult> AddUserImage(IFormFile Image)
        {
            try {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    return BadRequest(new ApiRespones(400));
                if (user.Image is not null)
                    ImageSetting.DeleteImage(user.Image, "Users"); 
                user.Image = ImageSetting.UplodaImage(Image, "Users");
                await _userManager.UpdateAsync(user);
    
                 return Ok(new ApiRespones(200, "Image Added"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));
            }
          
        }
        [HttpDelete("DeleteImage")]
        [Authorize]
        public async Task<ActionResult> DeleteImage()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    return BadRequest(new ApiRespones(400));
                if (user.Image is not null)
                    ImageSetting.DeleteImage(user.Image, "Users");
           
                await _userManager.UpdateAsync(user);
                ; return Ok(new ApiRespones(200, "Image Deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));
            }
        }
        [HttpGet("EmailExited")]
        public async Task<ActionResult<bool>> CheckEamil(string Email)
        {
            return await _userManager.FindByEmailAsync(Email) is not null;
        }
    }
}
