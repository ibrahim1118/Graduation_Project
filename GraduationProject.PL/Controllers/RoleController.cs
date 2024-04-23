using AutoMapper;
using DAL.AuthEntity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        public RoleController(RoleManager<IdentityRole> roleManager , 
            IMapper mapper , UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet("GetAll")]
       // [Authorize(Roles ="Admin")]
        public async Task<ActionResult> GetAll()
        {
            var roles = mapper.Map<IEnumerable<RoleDto>>(await roleManager.Roles.ToListAsync()); 
            return Ok(roles);
        }
        [HttpGet("GetRoleById")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> GetRoleById(string id)
        {
            var role = await roleManager.FindByIdAsync(id);  
            if (role is null)
                 return NotFound(new ApiRespones(404));
            var roleDto = mapper.Map<RoleDto>(role);
            return Ok(roleDto);
        }
        [HttpPost("AddRole")]
      //  [Authorize(Roles = "Admin")]
        public async Task <ActionResult> AddNewRole(RoleDto roleDto)
        {
            var role = await roleManager.RoleExistsAsync(roleDto.Name);
            if (!role)
            { await roleManager.CreateAsync(new IdentityRole { Name = roleDto.Name });
                return Ok(new ApiRespones(200, "Role Added")); 
            }
            else
            {
                return BadRequest(new ApiRespones(403, "Role is exists"));
            }
        }

        [HttpDelete("DeleteRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null)
                return NotFound(new ApiRespones(404, "Role Not Found"));

            try {
                await roleManager.DeleteAsync(role);
                return Ok(new ApiRespones(200, "Role Deleted"));
            }catch (Exception ex) 
            {
                return BadRequest(new ApiRespones(404, ex.Message));
            }
             
            }

        [HttpPut("Edit Role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditRole(RoleDto roleDto)
        {
            var role = await roleManager.FindByIdAsync(roleDto.Id);
            if (role == null)
                return NotFound(new ApiRespones(404, $"No Role with this {roleDto.Id}"));
            role.Name = roleDto.Name;
            try {
                await roleManager.UpdateAsync(role);
                return Ok(new ApiRespones(200, "Roel Has Updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRespones(404, ex.Message));
            }
        }

        [HttpPost("AddAdmin")]
       // [Authorize(Roles ="Admin")]
        public async Task<ActionResult> AddAdmin(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user is null)
                return NotFound(new ApiRespones(404, "User Not Fount"));
            var res =  await userManager.AddToRoleAsync(user, "Admin");
            if (res.Succeeded)
                return Ok(new ApiRespones(200, "add Successfully"));
            foreach (var error in res.Errors)
            {
                ModelState.AddModelError("", error.Description); 
            }
            return BadRequest(ModelState); 
        }

       

    }
}
