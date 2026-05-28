using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieRatingAPI.Data.Model;
using MovieRatingAPI.dto.auth;
using MovieRatingAPI.Interface;

namespace MovieRatingAPI.Controller
{
    [ApiController] // يفضل دائماً إضافة هذا الـ Attribute للـ APIs
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // تم تصحيح النوع هنا
        private readonly ITokenService _tokenService;

        // تم تعديل الـ Constructor لاستقبال الـ 3 خدمات وحقنهم بشكل صحيح
        public AuthController(
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, // إضافة الـ RoleManager هنا
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody ] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                UserName = dto.Username,
                Email = dto.Email,
       // تحويل الـ Enum إلى string ليطابق الكلاس
                Role = dto.Role.ToString()
            };

            // 1. إنشاء المستخدم بحساب كلمة المرور
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 2. تحويل الـ Enum إلى نص بأحرف صغيرة ليطابق الـ Roles المـبذورة
            string roleName = dto.Role.ToString().ToLower();

            // تأكد من أن الـ Role موجود في قاعدة البيانات قبل الإضافة
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
            else
            {
                return BadRequest(new { message = $"الدور {roleName} غير معرف في النظام مسبقاً." });
            }

            return Ok(new { message = "User Created Successfully and Role Assigned" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                return BadRequest("Invalid email or password");

            // توليد الـ JWT Token الذي تم تعديله في الخطوة السابقة ليحتوي على الـ Roles
            var token = _tokenService.GenerateToken(user);

            return Ok(new {
                userName = user.UserName,
                email = user.Email,
                Token = token
            });
        }
    }
}