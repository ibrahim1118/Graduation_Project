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
   
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController( IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<ActionResult> GetAll()
        {
            var category = await unitOfWork.Repostitry<Category>().GetAll();
            var categoryDto = _mapper.Map<IEnumerable<CategoryDto>>(category).ToList();
            return Ok(categoryDto);
        }

        [HttpPost("AddCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddCategory(CategoryDto categoryDto)
        {
            categoryDto.CreatedDate = DateTime.Now;
            var category = _mapper.Map<Category>(categoryDto);
            try {
                await unitOfWork.Repostitry<Category>().Add(category);
                return Ok(new ApiRespones(200, "Category has added Successfully"));
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            }
        [HttpDelete("DeleteCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await unitOfWork.Repostitry<Category>().GetById(id);
            if (category is null)
                return NotFound(new ApiRespones(404));
            try {
                await unitOfWork.Repostitry<Category>().Delete(category);
                return Ok(new ApiRespones(200, "Category has Deleted Successfully"));
            } catch (Exception ex)
            {
                return NotFound(new ApiRespones(404 , ex.Message));
            }
            
            }

        [HttpGet("GetCategoryById")]
        [Authorize]
        public async Task<ActionResult> GetTipByid(int id)
        {
            var tip = await unitOfWork.Repostitry<Category>().GetById(id);
            if (tip is null)
                return NotFound(new ApiRespones(404 , "This Category not Found"));
            var categoryDto = _mapper.Map<CategoryDto>(tip);
            return Ok(categoryDto);
        }

        [HttpPut("EditCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(CategoryDto categoryDto)
        {
            if (categoryDto.Id is null)
                return BadRequest(new ApiRespones(400));
            var category = await unitOfWork.Repostitry<Category>().GetById(categoryDto.Id.Value);
            if (category is null)
                return NotFound(new ApiRespones(404, $"No Category with {categoryDto.Id}"));
            category.Name = categoryDto.Name;
            category.CreatedDate = DateTime.Now;
            try {
                await unitOfWork.Repostitry<Category>().Update(category);
                return Ok(new ApiRespones(200, "Category has Updated Successfully"));
            }
             catch(Exception ex) { return BadRequest(
                 new ApiRespones(404 , ex.Message));}
            }
    }
}
