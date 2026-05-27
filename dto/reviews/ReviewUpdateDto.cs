using System.ComponentModel.DataAnnotations;

namespace MovieRatingAPI.Dto.Reviews;

public class ReviewUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
    public string Comment { get; set; } = string.Empty;
}