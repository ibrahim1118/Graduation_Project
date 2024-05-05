using Adminpanal.Hellper;
using AutoMapper;
using BLL.IRepository;
using BLL.Repository;
using DAL.AuthEntity;
using DAL.Entity;
using GraduationProject.API.DTOS;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepositry postRepo;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly ICommentRepositry commentRepositry;

        public PostController(IPostRepositry postRepo,
            IMapper mapper,
            IConfiguration configuration,
            UserManager<AppUser> userManager , 
            ICommentRepositry commentRepositry)
        {
            this.postRepo = postRepo;
            this.mapper = mapper;
            this.configuration = configuration;
            this.userManager = userManager;
            this.commentRepositry = commentRepositry;
        }
        [HttpGet("GetAllPosts")]
        public async Task<ActionResult> GetAllPost()
        {
            try
            {
                var posts = await postRepo.GetAll();
                foreach (var post in posts)
                {
                    post.Comments.RemoveWhere(c => c.ParentId is not null);
                }
                var pstdto = mapper.Map<IEnumerable<GetPostDto>>(posts);
                return Ok(pstdto);
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message)); 
            }
        }
        [HttpPost("AddReact")]
        [Authorize]
        public async Task<ActionResult> AddReact(ReactDto reactDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);
                if (user is null)
                    return BadRequest(new ApiRespones(400, "User Not Found"));
                var pos = await postRepo.GetById(reactDto.ObjectId);
                if (pos is null)
                {
                    return BadRequest(new ApiRespones(403, "No post with this id"));
                }
                if (reactDto.like && reactDto.dislike)
                {
                    return BadRequest(new ApiRespones(403, "InValidRequest"));
                }
                var react = new PostReact()
                {
                    PostId = reactDto.ObjectId,
                    userId = userId,
                    like = reactDto.like,
                    disLike = reactDto.dislike
                };
                await postRepo.AddReact(react);
                var post = await postRepo.GetById(reactDto.ObjectId);
                var postDto = mapper.Map<GetPostDto>(post);
                return Ok(postDto);
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));
            }

        }
        [HttpPost("AddPost")]
        [Authorize]
        public async Task<ActionResult> AddPost([FromForm]AddPostDto posdto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return BadRequest(new ApiRespones(400, "User Not Found"));
            string? ImageName = null; 
            if (posdto.Image is not null)
            {
                ImageName = ImageSetting.UplodaImage(posdto.Image, "Post"); 
            }
            var post = new Post()
            {
                Content = posdto.Content,
                Image = ImageName,
                AppUserId = userId,
                likes = 0 , 
                DisLikes = 0 ,
                CreationDate = DateTime.Now,
                
            };
            try
            {
                await postRepo.Add(post);
                return Ok(new ApiRespones(200, "Post Added")); 
            }
            catch (Exception ex)
            {
               return BadRequest(new ApiRespones(404 , ex.Message));
            }

          
        }
        [HttpGet("GetPostById")]
        public async Task<ActionResult> GetPostById(int id)
        {
            try
            {
                var post = await postRepo.GetById(id);
                if (post is null)
                    return NotFound(new ApiRespones(404, $"no Post with this id {id}"));
                post.Comments.RemoveWhere(com => com.ParentId != null);
                var postDto = mapper.Map<GetPostDto>(post);
                return Ok(postDto);
            }catch (Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));
            }
        }
        [HttpPut("EditPost")]
        [Authorize]
        public async Task<ActionResult> EditPost(AddPostDto postdto)
        {
            try
            {
                var post = await postRepo.GetById(postdto.Id);
                if (post is null)
                    return NotFound(new ApiRespones(404, "Not Found"));
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != post.AppUserId)
                    return Unauthorized(new ApiRespones(401, "Not Authorized"));
                if (postdto.Image is not null)
                {
                    ImageSetting.DeleteImage(post.Image, "Post");
                    post.Image = ImageSetting.UplodaImage(postdto.Image, "Post");
                }
                if (postdto.Content is not null)
                    post.Content = postdto.Content;
                await postRepo.Update(post);
                return Ok(new ApiRespones(200, "Post Updated"));
                //return Ok(post);
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message));

            }
        }
        [HttpDelete("DeletePost")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await postRepo.GetById(id);
            if (post is null)
                return NotFound(new ApiRespones(404, "Not Fount")); 
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user is null)
                    return BadRequest(new ApiRespones(400, "InValid Token")); 
                var rols = await userManager.GetRolesAsync(user);
                if (userId!=post.AppUserId&&!rols.Any(r=> r=="Admin"))
                   return Unauthorized(new ApiRespones(401 , "Not Authorized"));
                if (post.Image is not null)
                ImageSetting.DeleteImage(post.Image, "Post");
                foreach (var react in post.postReacts)
                    await postRepo.DeleteReact(react); 
                foreach(var comment in post.Comments)
                {
                    if (comment.comments.Count > 0)
                    { foreach (var comment2 in comment.comments)
                            await commentRepositry.Delete(comment2);
                    }
                   await commentRepositry.Delete(comment); 
                }
                await postRepo.Delete(post);
                return Ok(new ApiRespones(200, "Post Deleted")); 
            }catch (Exception ex)
            {
                return BadRequest(new ApiRespones(400 , ex.Message));
            }
        }
        

    }
}
