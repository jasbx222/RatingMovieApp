using Microsoft.AspNetCore.Mvc;
using MovieRatingAPI.Interface;
using MovieRatingAPI.Dto.movies;
using MovieRatingAPI.Data;
// using Microsoft.AspNetCore.Mvc.NotFoundObjectResult;
using MovieRatingAPI.Dto.Reviews;
using Microsoft.AspNetCore.Http.HttpResults;
using MovieRatingAPI.Data.Model;
using Microsoft.AspNetCore.Authorization;

namespace MovieRatingAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
      [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsInterface _revies;

        public ReviewsController(IReviewsInterface reviewsInterface)
        {
            _revies = reviewsInterface;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetAll()
        {
            var reviews = await _revies.GetAllAsync();

            return Ok(reviews);
        }




  
[HttpPost]
public async Task<ActionResult<ReviewResponseDto>> Create([FromBody] ReviewCreateDto reviewCreateDto)
{
    // Note: In ASP.NET Core [ApiController], ModelState validation is handled automatically, 
    // but keeping it explicit is perfectly fine if you prefer.
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // 1. Map the DTO to the Domain Entity (Including the missing MovieId)
    var review = new Reviews
    {
        MovieId = reviewCreateDto.MovieId,
        UserName = reviewCreateDto.UserName,
        Rating = reviewCreateDto.Rating,
        Comment = reviewCreateDto.Comment
        // CreatedAt is set automatically by your model default (= DateTime.UtcNow)
    };

    // 2. Save to the database via repository
    var createdReview = await _revies.CreateAsync(review);

   

    // 4. Return CreatedAtAction (Best practice for POST) or Ok(responseDto)
    return Ok("تمت الاضاقة بنجاح");
}
        [HttpPut("{id}")]


        public async Task<ActionResult<ReviewResponseDto>> Update(int id, [FromBody] ReviewUpdateDto reviewUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedReview= await _revies.UpdateAsync(id, reviewUpdateDto);

            if (updatedReview == null)
                return NotFound();

            return Ok(updatedReview);
        }



        [HttpGet("{id}")]


        public async Task<Reviews?> GetByIdAsync(int id)
        {
            var review = await _revies.GetByIdAsync(id);
            return review;


        }
        [HttpDelete("{id}")]


        public async Task<Reviews?> DeleteAsync(int id)
        {
             await _revies.DeleteAsync(id);
            return null;


        }
    }
}