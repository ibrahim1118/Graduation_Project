using Adminpanal.Hellper;
using AutoMapper;
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
       
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        public CommentController(IUnitOfWork unitOfWork,
            IMapper mapper 
            ,UserManager<AppUser> userManager)
        {
           
            
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet("GetAllComment")]
        public async Task<ActionResult> GetAllComment()
        {
            try
            {
                 
                var comment = (List<Comment>)await unitOfWork.CommentRepositry().GetAll();
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
            var post = await unitOfWork.PostRepositry().GetById(commentDto.PostId);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return BadRequest(new ApiRespones(400, "User Not Found")); 
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
                  await  unitOfWork.CommentRepositry().AddReplie(comment);
                else 
                await unitOfWork.CommentRepositry().Add(comment);
                return Ok(new ApiRespones(200, "Comment Added")); 

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCommentByPostId")]
        public async Task<ActionResult> GetCommentByPostId(int postId  , string UserId)
        {
            try
            {
                var comments = await unitOfWork.CommentRepositry().GetCommentsByPostId(postId);
                if (comments is null)
                {
                    return NotFound(new ApiRespones(404, "Post Not Fount"));


                }
                var comment = mapper.Map<IEnumerable<GetCommentDto>>(comments);
                var rect = unitOfWork.CommentRepositry().GetAllReact();
                foreach (var com in comment)
                {
                    var rec = rect.FirstOrDefault(r => r.userId == UserId && r.CommentId == com.Id);
                    if (rec is not null)
                    {
                        com.IsLiked = rec.like;
                        com.IsDisliked = rec.disLike; 
                    }
                }
                return Ok(comment);
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message)); 
            }
        }

        [HttpPost("AddReact")]
        [Authorize]
        public async Task<ActionResult> AddReact(ReactDto reactDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return BadRequest(new ApiRespones(400, "User Not Found"));
            var comment = await unitOfWork.CommentRepositry().GetById(reactDto.ObjectId);
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
                await unitOfWork.CommentRepositry().AddReact(react);
                comment = await unitOfWork.CommentRepositry().GetById(reactDto.ObjectId);
                var commentDto = mapper.Map<GetCommentDto>(comment);
                var rect = unitOfWork.CommentRepositry().GetReact(reactDto.ObjectId, userId); 
                return Ok(new
                {
                    id = comment.Id , 
                    likes = comment.likes, 
                    dislikes = comment.DisLikes, 
                    IsLike = rect!=null? rect.like : false,
                    IDisLike = rect!= null? rect.disLike: false

                });
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
            var comment = await unitOfWork.CommentRepositry().GetById(id);
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
                    await unitOfWork.CommentRepositry().DeleteReact(react);
                foreach (var item in comment.comments)
                {
                    if (item.Image is not null)
                    ImageSetting.DeleteImage(item.Image, "Comment"); 
                    await unitOfWork.CommentRepositry().Delete(item);
                }
                await  unitOfWork.CommentRepositry().Delete(comment);
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
            var comment = await unitOfWork.CommentRepositry().GetById(commentDto.Id);
            if (comment is null)
                return NotFound(new ApiRespones(404, "Not Found"));
            if (userId != comment.AppUserId)
            {
                return Unauthorized(new ApiRespones(401, "Not Authorize")); 
            }
            if (commentDto.Image is not null)
            {
                if (comment.Image is not null)
                ImageSetting.DeleteImage(comment.Image, "Comment");
                comment.Image = ImageSetting.UplodaImage(commentDto.Image, "Comment");
            }
            if (commentDto.Content is not null)
                comment.Content = commentDto.Content;
            await unitOfWork.CommentRepositry().Update(comment);
            return Ok(comment);
        }
        [HttpDelete("DeleteCommentImage")]
        [Authorize]
        public async Task<ActionResult> DeletePostImage(int commentId)
        {
            try
            {

                var comment = await unitOfWork.CommentRepositry().GetById(commentId);
                if (comment is null)
                    return NotFound(new ApiRespones(404, "Not Found"));
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != comment.AppUserId)
                    return Unauthorized(new ApiRespones(401, "Not Authorized"));
                if (comment.Image != null)
                {
                    ImageSetting.DeleteImage(comment.Image, "Post");
                    comment.Image = null;
                    await unitOfWork.CommentRepositry().Update(comment);
                }
                return Ok(new ApiRespones(200, "Image Deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));
            }

        }

    }
}
