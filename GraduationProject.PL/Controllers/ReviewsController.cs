using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using GraduationProject.API.DTOS;
using BLL.IRepository;
using DAL.Entity;
using GraduationProject.API.ErrorsHandl;
using Microsoft.AspNetCore.Authorization;

namespace GraduationProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IReviewRepository _userReviewRepo;

        public ReviewsController(HttpClient httpClient , IReviewRepository userReviewRepo)
        {
            _httpClient = httpClient;
            _userReviewRepo = userReviewRepo;
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(UserReviewDto Reviews)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://sentemament-analycis.onrender.com/");
                var Json = JsonSerializer.Serialize(Reviews);
                var Content = new StringContent(Json, Encoding.UTF8, "application/json");
                var res = client.PostAsync("predict", Content).Result;
                if (res.IsSuccessStatusCode)
                {
                    var ress = res.Content.ReadAsStringAsync().Result;
                    var ob = JsonSerializer.Deserialize<predictionRes>(ress);
                    await _userReviewRepo.Add(new UserReview()
                    {
                        review = Reviews.Reviews,
                        type = ob.prediction
                    });
                    var pos = await _userReviewRepo.GetReviewsbyType("Positive");
                    var Neg =await _userReviewRepo.GetReviewsbyType("Negative");
                    var all = await _userReviewRepo.GetAll(); 
                    return Ok(new
                    {
                        Positive = $"{((decimal) pos.Count()/ all.Count())*100}%",
                        Negative = $"{((decimal)Neg.Count() / all.Count()) * 100}%"
                    });
                }
                else
                    return BadRequest(new ApiRespones(400 ,res.Content.ToString()));
            }catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400  , ex.Message));
            }
        }
        [HttpGet("GetAllReviwes")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllReviwes()
        {
            try
            {
                var rev = await _userReviewRepo.GetAll();
                var res = rev.Select(r => new ReviewsDto()
                {
                    Reviews = r.review,
                    Type = r.type,
                }); 
                return Ok(res); 
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiRespones(400, ex.Message)); 
            }
        }
    }
}
