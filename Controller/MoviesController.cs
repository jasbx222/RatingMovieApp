using Microsoft.AspNetCore.Mvc;
using MovieRatingAPI.Interface;
using MovieRatingAPI.Dto.movies;
using MovieRatingAPI.Data;
// using Microsoft.AspNetCore.Mvc.NotFoundObjectResult;
using MovieRatingAPI.Dto.Reviews;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Hangfire;
using Serilog;
using MovieRatingAPI.dto.email;
using MovieRatingAPI.Helpers;

namespace MovieRatingAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="admin")]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesInterface _movies;
        private readonly IEmailInterface _emailService;

        public MoviesController(IMoviesInterface moviesInterface, IEmailInterface emailService)
        {
            _movies = moviesInterface;
            _emailService = emailService;
        }
   
        [HttpGet]


        public async Task<ActionResult<IEnumerable<MovieResponseDto>>> GetAll([FromQuery] QueryObject queryObject)
        {
            var movies = await _movies.GetAllAsync(queryObject);

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

            // Fire and forget


            var JopsIdOne = BackgroundJob.Enqueue(() => _emailService.SendEmailAsync());
            // Schedule & deployment 
            // var JopsIdTwo = BackgroundJob.Schedule(() => GetAll(), TimeSpan.FromSeconds(5));
            // ContinueJobWith jop worked by action like another jop work
            // BackgroundJob.ContinueJobWith(1, () => Log.Warning($" ContinueJobWith worked {1}"));

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

[Authorize(Policy = "DeleteMovie")]
        public async Task<Movies?> DeleteAsync(int id)
        {
            await _movies.DeleteAsync(id);
            return null;


        }




    }
}