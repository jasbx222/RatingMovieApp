using Microsoft.AspNetCore.Mvc;
using MovieRatingAPI.dto.email;

namespace MovieRatingAPI.Interface

{
public interface IEmailInterface 
{
    public Task SendEmailAsync ();
}

}