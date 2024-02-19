using DAL.AuthEntity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using GraduationProject.API.Helpear;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Service.ImpService;
using Service.IService;
namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;

        public AccountController(UserManager<AppUser> userManager ,
            SignInManager<AppUser> signInManager, 
             ITokenService tokenService)
        {
            _userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
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
                return NotFound("This user is not Fount"); 
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

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            

                var user = await _userManager.FindByEmailAsync(model.Email);
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                    return Ok("Password Changed Success");

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

             return BadRequest(ModelState);
            
        }  
         [HttpGet("EmailExited")]
        public async Task<ActionResult<bool>> ChackEamil(string Email)
        {
            return await _userManager.FindByEmailAsync(Email) is not null;
        }
    }
}
