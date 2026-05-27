using System.Collections.Generic;
using System.Threading.Tasks;
using MovieRatingAPI.Data;
using MovieRatingAPI.Data.Model;
using MovieRatingAPI.Dto.Reviews;


namespace MovieRatingAPI.Interface
{
    public interface IReviewsInterface
    {
        Task<IEnumerable<Reviews>> GetAllAsync();
        Task<Reviews?> GetByIdAsync(int id);
        Task<Reviews> CreateAsync(Reviews review);
        
        Task<Reviews?>DeleteAsync(int id);
        Task<ReviewResponseDto?> UpdateAsync(int id ,ReviewUpdateDto reviewUpdateDto);
    }
}
