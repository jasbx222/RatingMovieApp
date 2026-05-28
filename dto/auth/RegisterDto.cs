using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MovieRatingAPI.Helpers.Enum;

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
[Required]
public UserRole Role { get; set; } // اجعل النوع هو الـ enum الخاص بك مباشرة
}