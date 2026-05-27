using System.ComponentModel.DataAnnotations;

namespace MovieRatingAPI.Dto.movies;

public class NewMovieDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Year must be 4 digits")]
    public string ReleaseYear { get; set; } = string.Empty;


    
}