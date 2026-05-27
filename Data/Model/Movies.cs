using MovieRatingAPI.Data.Model;

namespace MovieRatingAPI.Data;


public class Movies
{


    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Genre { get; set; }

    public string Description { get; set; }

    public DateTime ReleaseYear { get; set; } = DateTime.UtcNow;
   public List<Reviews> Reviews { get; set; } = new();



 

}