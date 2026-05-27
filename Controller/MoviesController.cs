using Microsoft.AspNetCore.Mvc;
using MovieRatingAPI.Interface;
using MovieRatingAPI.Dto.movies;
using MovieRatingAPI.Data;
// using Microsoft.AspNetCore.Mvc.NotFoundObjectResult;
using MovieRatingAPI.Dto.Reviews;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace MovieRatingAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesInterface _movies;

        public MoviesController(IMoviesInterface moviesInterface)
        {
            _movies = moviesInterface;
        }

        [HttpGet]
     
        public async Task<ActionResult<IEnumerable<MovieResponseDto>>> GetAll()
        {
            var movies = await _movies.GetAllAsync();

            return Ok(movies);
        }




        [HttpPost]

        public async Task<ActionResult<NewMovieDto>> Create([FromForm] NewMovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var movie = new Movies()
            {

                Title = movieDto.Title,
                Description = movieDto.Description,
                Genre = movieDto.Genre,

            };

            var createdMovie = await _movies.CreateAsync(movie);

            return Ok();
        }
        [HttpPut("{id}")]


        public async Task<ActionResult<UpdateMovieDto>> Update(int id, [FromBody] UpdateMovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedMovie = await _movies.UpdateAsync(id, movieDto);

            if (updatedMovie == null)
                return NotFound();

            return Ok(updatedMovie);
        }



        [HttpGet("{id}")]


        public async Task<Movies?> GetByIdAsync(int id)
        {
            var movie = await _movies.GetByIdAsync(id);
            return movie;


        }
        [HttpDelete("{id}")]


        public async Task<Movies?> DeleteAsync(int id)
        {
             await _movies.DeleteAsync(id);
            return null;


        }
    }
}