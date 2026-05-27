namespace MovieRatingAPI.Data.Model;


public class Reviews
{
    public int Id { get; set; }

    public int MovieId { get; set;}

    public Movies Movie {get;set;}

    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;




}