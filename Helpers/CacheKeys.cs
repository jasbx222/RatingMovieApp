namespace MovieRatingAPI.Helpers;



public class CacheKeys
{
    



    public static string  GetAllMovies = "movies_all";
    public static string GetMovieById(int id)
    {
        return $"movie_{id}";
    }

}