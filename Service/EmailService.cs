using MailKit.Net.Smtp;
using MimeKit;
using MovieRatingAPI.Interface;

public class EmailService : IEmailInterface
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync()
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("Movie API", "test@mailtrap.io"));

        email.To.Add(MailboxAddress.Parse("test@gmail.com"));

        email.Subject = "Hello";

        email.Body = new TextPart("html")
        {
            Text = "<h1>Movie API</h1>"
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _config["EmailSettings:Host"],
            int.Parse(_config["EmailSettings:Port"]),
            MailKit.Security.SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _config["EmailSettings:Email"],
            _config["EmailSettings:Password"]
        );

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}