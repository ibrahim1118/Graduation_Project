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
    public class CategoryController : ControllerBase
    {
        private readonly IGenricRepository<Category> _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryController(IGenricRepository<Category> _CategoryRepo,
            IMapper mapper)
        {
            _categoryRepo = _CategoryRepo;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var category = await _categoryRepo.GetAll();
            var categoryDto = _mapper.Map<IEnumerable<CategoryDto>>(category).ToList();
            return Ok(categoryDto);
        }

        [HttpPost("AddCategory")]
        public async Task<ActionResult> AddCategory(CategoryDto categoryDto)
        {
            categoryDto.CreatedDate = DateTime.Now;
            var category = _mapper.Map<Category>(categoryDto);
            await _categoryRepo.Add(category);
            return Ok(new ApiRespones(200 , "Category has added successful"));
        }
        [HttpDelete("DeleteCategory")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepo.GetById(id);
            if (category is null)
                return NotFound(new ApiRespones(404));
            await _categoryRepo.Delete(category);
            return Ok(new ApiRespones(200 , "Category has Deleteed successful"));
        }

        [HttpGet("GetCategoryByid")]
        public async Task<ActionResult> GetTipByid(int id)
        {
            var tip = await _categoryRepo.GetById(id);
            if (tip is null)
                return NotFound(new ApiRespones(404 , "This Category not Found"));
            var categoryDto = _mapper.Map<CategoryDto>(tip);
            return Ok(categoryDto);
        }

        [HttpPut("EditCategory")]

        public async Task<ActionResult> Edit(CategoryDto categoryDto)
        {
            if (categoryDto.Id is null)
                return BadRequest(new ApiRespones(400));
            var category = await _categoryRepo.GetById(categoryDto.Id.Value);
            if (category is null)
                return NotFound(new ApiRespones(404 , $"No Category with {categoryDto.Id}"));
            category.Name = categoryDto.Name;
            category.CreatedDate = DateTime.Now;
            await _categoryRepo.Update(category);
            return Ok(new ApiRespones(200 , "Category has Updated successful"));
        }
    }
}
