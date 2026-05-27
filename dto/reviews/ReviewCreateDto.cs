using System.ComponentModel.DataAnnotations;

namespace MovieRatingAPI.Dto.Reviews;

public class ReviewCreateDto
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
    public string Comment { get; set; } = string.Empty;
}