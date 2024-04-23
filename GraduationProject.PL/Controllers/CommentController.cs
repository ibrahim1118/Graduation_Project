using Adminpanal.Hellper;
using AutoMapper;
using BLL.IRepository;
using DAL.AuthEntity;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepositry commentRepo;
        private readonly IMapper mapper;
        private readonly IGenricRepository<Post> postRepo;
        private readonly UserManager<AppUser> userManager;

        public CommentController(ICommentRepositry commentRepo,
            IMapper mapper , 
            IGenricRepository<Post> PostRepo
            ,UserManager<AppUser> userManager)
        {
           
            this.commentRepo = commentRepo;
            this.mapper = mapper;
            postRepo = PostRepo;
            this.userManager = userManager;
        }

        [HttpGet("GetAllComment")]
        public async Task<ActionResult> GetAllComment()
        {
            try
            {
               
                var comment = (List<Comment>)await commentRepo.GetAll();
                comment.RemoveAll(c => c.ParentId is not null);
                var respone = mapper.Map<IEnumerable<GetCommentDto>>(comment);
                return Ok(respone);
            }catch (Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));
            }
            
        }
        [HttpPost("AddComment")]
        [Authorize]
        public async Task<ActionResult> AddComment(AddCommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = postRepo.GetById(commentDto.PostId); 
            if (post is null)
            {
                return BadRequest(new ApiRespones(500, "inCorrect post id ")); 
            }
            string? ImageName = null;
            if (commentDto.Image is not null)
            {
                ImageName = ImageSetting.UplodaImage(commentDto.Image, "Comment");
            }
            var comment = new Comment()
            {
                Id = commentDto.Id,
                Content = commentDto.Content,
                Image = ImageName,
                likes = 0, 
                DisLikes =0,
                AppUserId = userId,
                ParentId  = commentDto.ParentId,
                PostId = commentDto.PostId,

            };
            try
            {
                if (comment.ParentId is not null)
                  await   commentRepo.AddReplie(comment);
                else 
                await commentRepo.Add(comment);
                return Ok(new ApiRespones(200, "Comment Added")); 

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddReact")]
        [Authorize]
        public async Task<ActionResult> AddReact(ReactDto reactDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await commentRepo.GetById(reactDto.ObjectId);
            if (comment is null)
            {
                return BadRequest(new ApiRespones(400, "No post with this id"));
            }
            if (reactDto.like && reactDto.dislike)
            {
                return BadRequest(new ApiRespones(400, "InValidRequest"));
            }
            var react = new CommentReact()
            {
                CommentId = reactDto.ObjectId,
                userId = userId,
                like = reactDto.like,
                disLike = reactDto.dislike
            };
            try
            {
                await commentRepo.AddReact(react);
                comment = await commentRepo.GetById(reactDto.ObjectId);
                var commentDto = mapper.Map<GetCommentDto>(comment);
                return Ok(commentDto);
            }catch (Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));
            }


        }
        [HttpDelete("DeleteComment")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await commentRepo.GetById(id);
            if (comment is null)
                return NotFound(new ApiRespones(404));
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                var rols = await userManager.GetRolesAsync(user);
                if (userId != comment.AppUserId && !rols.Any(r => r == "Admin"))
                    return Unauthorized(new ApiRespones(401, "Not Authorized"));
                if (comment.Image is not null)
                    ImageSetting.DeleteImage(comment.Image, "Comment");
                foreach (var react in comment.Reactes)
                    await commentRepo.DeleteReact(react);
                foreach (var item in comment.comments)
                {
                    if (item.Image is not null)
                    ImageSetting.DeleteImage(item.Image, "Comment"); 
                    await commentRepo.Delete(item);
                }
                await commentRepo.Delete(comment);
                return Ok(new ApiRespones(200, "Comment Deleted"));
            } catch (Exception ex)
            {
                return BadRequest(new ApiRespones(404, ex.Message));
            }

        }
        [HttpPut("EditComment")]
        [Authorize]
        public async Task<ActionResult> EditComment(AddCommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await commentRepo.GetById(commentDto.Id);
            if (comment is null)
                return NotFound(new ApiRespones(404, "Not Found"));
            if (userId != comment.AppUserId)
            {
                return Unauthorized(new ApiRespones(401, "Not Authorize")); 
            }
            if (commentDto.Image is not null)
            {
                ImageSetting.DeleteImage(comment.Image, "Comment");
                comment.Image = ImageSetting.UplodaImage(commentDto.Image, "Comment");
            }
            if (commentDto.Content is not null)
                comment.Content = commentDto.Content;
            await commentRepo.Update(comment);
            return Ok(comment);
        }
   
    }
}
