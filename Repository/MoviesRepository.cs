using MovieRatingAPI.Data;
using MovieRatingAPI.Interface;
using MovieRatingAPI.Dto.movies;
using Microsoft.EntityFrameworkCore;
using MovieRatingAPI.Dto.Reviews;
using MovieRatingAPI.Helpers;

namespace MovieRatingAPI.Repository;

public class MoviesRepository : IMoviesInterface
{
    private readonly DataContext _context;
    private readonly ICacheService _cacheService;

    public MoviesRepository(DataContext dataContext, ICacheService cacheService)
    {
        _context = dataContext;
        _cacheService = cacheService;
    }

    // =========================
    // CREATE
    // =========================
    public async Task<Movies> CreateAsync(Movies movie)
    {
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveData(CacheKeys.GetAllMovies);

        return movie;
    }

    // =========================
    // DELETE
    // =========================
    public async Task<Movies?> DeleteAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
            return null;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveData(CacheKeys.GetAllMovies);
        await _cacheService.RemoveData(CacheKeys.GetMovieById(id));

        return movie;
    }

    // =========================
    // GET ALL
    // =========================
   public async Task<IEnumerable<MovieResponseDto>> GetAllAsync(QueryObject queryObject)
{
    var cachedMovies =
        await _cacheService.GetData<List<MovieResponseDto>>(CacheKeys.GetAllMovies);

    // =========================
    // FROM REDIS
    // =========================
    if (cachedMovies != null)
    {
        Console.WriteLine("🔥 Data loaded FROM REDIS CACHE");

        // الأفضل احترافيًا:
        // _logger.LogInformation("Data loaded FROM REDIS CACHE");

        return cachedMovies;
    }

    Console.WriteLine("⚡ Cache MISS - Loading FROM DATABASE");

    var query = _context.Movies
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(queryObject.MovieName))
    {
        query = query.Where(m => m.Title.Contains(queryObject.MovieName));
    }

    var movieDtos = await query
        .Select(m => new MovieResponseDto
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
        })
        .ToListAsync();

    await _cacheService.SetData(
        CacheKeys.GetAllMovies,
        movieDtos,
        TimeSpan.FromMinutes(10));

    Console.WriteLine("💾 Data SAVED to REDIS CACHE");

    return movieDtos;
}

    // =========================
    // GET BY ID
    // =========================
    public async Task<Movies?> GetByIdAsync(int id)
    {
        var cacheKey = CacheKeys.GetMovieById(id);

        var cachedMovie =
            await _cacheService.GetData<Movies>(cacheKey);

        if (cachedMovie != null)

       { 
           Console.WriteLine("redis is worked ");

            return cachedMovie;
       
}
         

        var movie = await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
            return null;

        await _cacheService.SetData(
            cacheKey,
            movie,
            TimeSpan.FromMinutes(10));

        return movie;
    }

    // =========================
    // UPDATE
    // =========================
    public async Task<UpdateMovieDto?> UpdateAsync(int id, UpdateMovieDto dto)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
            return null;

        movie.Title = dto.Title;
        movie.Description = dto.Description;
        movie.Genre = dto.Genre;

        await _context.SaveChangesAsync();

        await _cacheService.RemoveData(CacheKeys.GetAllMovies);
        await _cacheService.RemoveData(CacheKeys.GetMovieById(id));

        return new UpdateMovieDto
        {
            Title = movie.Title,
            Genre = movie.Genre,
            Description = movie.Description
        };
    }
}