using System.ComponentModel.DataAnnotations;

namespace MovieRatingAPI.Dto.Reviews;

public class UpdateMovieDto
{
    
    public string Title { get; set; } = string.Empty;

   
    public string Genre { get; set; } = string.Empty;

    
    public string Description { get; set; } = string.Empty;
    public string ReleaseYear { get; set; } = string.Empty;

}