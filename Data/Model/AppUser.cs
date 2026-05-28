using Microsoft.AspNetCore.Identity;

namespace MovieRatingAPI.Data.Model;



public class AppUser : IdentityUser
{
    
    public string Role {get;set;}
}