using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MovieRatingAPI.Data;
using MovieRatingAPI.Data.Model;
using MovieRatingAPI.Dto.Reviews;
using MovieRatingAPI.Interface;

namespace MovieRatingAPI.Repository;


public class ReviewRepository : IReviewsInterface
{


    private readonly DataContext _context;

    public ReviewRepository(DataContext dataContext)
    {
        _context = dataContext;
    }
    public async Task<Reviews> CreateAsync(Reviews review)
    {

        await _context.Reviews.AddAsync(review);

        await _context.SaveChangesAsync();

        return review;



    }

    public async Task<Reviews?> DeleteAsync(int id)
    {

          var review = await _context.Reviews.FindAsync(id);

    // 2. If it doesn't exist, return null so the controller can return a 404
    if (review == null)
    {
        return null;
    }

    // 3. FIX: Use the Reviews DbSet instead of Movies
    _context.Reviews.Remove(review);
    
    // 4. Persist changes to the database
    await _context.SaveChangesAsync();
    
    // 5. Return the deleted review object
    return null;
    }
    

    public async Task<IEnumerable<Reviews>> GetAllAsync()
    {
        var reviews = await _context.Reviews.ToListAsync();

        return reviews;
    }

    public async Task<Reviews?> GetByIdAsync(int id)
    {
        var reviews = await _context.Reviews.FindAsync(id);


        return reviews;
    }

    public async Task<ReviewResponseDto?> UpdateAsync(int id, ReviewUpdateDto reviewUpdateDto)
    {

        var review = await _context.Reviews.FindAsync(id);

        // 2. Return null if it doesn't exist (Controller will turn this into a 404 Not Found)
        if (review == null)
            return null;

        // 3. Update only the allowed fields from the DTO
        review.Rating = reviewUpdateDto.Rating;
        review.Comment = reviewUpdateDto.Comment;

        // 4. Save changes to the database
        await _context.SaveChangesAsync();

        // 5. Return the updated data mapped to a Response DTO
        return new ReviewResponseDto
        {
            Id = review.Id,
            MovieId = review.MovieId,
            UserName = review.UserName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };

    }
}