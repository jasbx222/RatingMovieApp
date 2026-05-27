using MovieRatingAPI.Dto.Reviews;

namespace MovieRatingAPI.Dto.movies;

public class MovieResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    
    // هنا نضع الـ DTO المبسط للتقييمات وليس الكائن الأصلي لمنع التكرار الدائري
    public IEnumerable<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
}