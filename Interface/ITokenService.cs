using MovieRatingAPI.Data.Model;

namespace MovieRatingAPI.Interface;



public interface ITokenService
{
    public string GenerateToken(AppUser user);
}