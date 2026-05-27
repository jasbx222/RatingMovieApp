using MovieRatingAPI.Data;
using MovieRatingAPI.Interface;
using MovieRatingAPI.Dto.movies;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using MovieRatingAPI.Dto.Reviews;
using Microsoft.AspNetCore.Http.HttpResults;
namespace MovieRatingAPI.Repository;


public class MoviesRepository : IMoviesInterface
{
    private readonly DataContext _context;

    public MoviesRepository(DataContext dataContext)
    {
        _context = dataContext;
    }
    public async Task<Movies> CreateAsync(Movies movie)
    {

        await _context.Movies.AddAsync(movie);

        await _context.SaveChangesAsync();


        return movie;
    }

    public async Task<Movies?> DeleteAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return null;
        }



        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return null;



    }

    public async Task<IEnumerable<MovieResponseDto>> GetAllAsync()
    {
        var movies = await _context.Movies.Include(r => r.Reviews).ToListAsync();

     var movieDtos = movies.Select(m => new MovieResponseDto
    {
        Id = m.Id,
        Title = m.Title,
        Genre = m.Genre,
        Reviews = m.Reviews.Select(r => new ReviewResponseDto
        {
            Id = r.Id,
            MovieId = r.MovieId,
            UserName = r.UserName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList()
    });

    return movieDtos;
    }

    public async Task<Movies?> GetByIdAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        return movie;
    }

    public async Task<UpdateMovieDto?> UpdateAsync(int id, UpdateMovieDto dto)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
            return null;

        movie.Title = dto.Title;
        movie.Description = dto.Description;
        movie.Genre = dto.Genre;

        await _context.SaveChangesAsync();

        return new UpdateMovieDto
        {
            // Id = movie.Id,
            Title = movie.Title,
            Genre = movie.Genre,
            // ReleaseYear = movie.ReleaseYear
        };
    }
}