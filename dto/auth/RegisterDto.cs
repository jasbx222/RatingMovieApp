using System.ComponentModel.DataAnnotations;

namespace MovieRatingAPI.dto.auth;

public class RegisterDto
{
  public string  Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}