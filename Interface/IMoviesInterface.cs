using System.Collections.Generic;
using System.Threading.Tasks;
using MovieRatingAPI.Data;
using MovieRatingAPI.Dto.movies;
using MovieRatingAPI.Dto.Reviews;
using MovieRatingAPI.Helpers;


namespace MovieRatingAPI.Interface
{
    public interface IMoviesInterface
    {
        Task<IEnumerable<MovieResponseDto>> GetAllAsync(QueryObject queryObject);
        Task<Movies?> GetByIdAsync(int id);

        Task<Movies?>DeleteAsync(int id);
        Task<Movies> CreateAsync(Movies movie);
        Task<UpdateMovieDto?> UpdateAsync(int id, UpdateMovieDto movieDto);
    }
}
