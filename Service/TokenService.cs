namespace MyApi.Service;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity; // تأكد من إضافة هذا الـ namespace
using MovieRatingAPI.Data.Model;
using MovieRatingAPI.Interface;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager; // 1. إضافة الـ UserManager

    // 2. حقن الـ UserManager في الـ Constructor
    public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public string GenerateToken(AppUser user)
    {
        // 3. جلب الأدوار الخاصة بالمستخدم بشكل متزامن
        var userRoles = _userManager.GetRolesAsync(user).Result;

        // 4. تحويل الـ Claims إلى List لكي نتمكن من الإضافة عليها ديناميكياً
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 5. إضافة الأدوار المسترجعة إلى قائمة الـ Claims
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims, // نمرر القائمة الجديدة التي تحتوي على الأدوار
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}