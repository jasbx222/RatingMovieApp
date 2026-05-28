using Microsoft.EntityFrameworkCore;
using Serilog;
using MovieRatingAPI.Data;
using MovieRatingAPI.Interface;
using MovieRatingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using MovieRatingAPI.Data.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyApi.Service; // تأكد أن هذا الـ Namespace مطابق لكلاس الـ TokenService الخاص بك
using Microsoft.OpenApi.Models;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// ================= Serilog Configuration =================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ================= Controllers =================
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        // هذا السطر يحول الـ Enums من أرقام إلى نصوص في الـ JSON
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// ================= Swagger (With JWT Bearer Config) =================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movie Rating API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ================= DB Context =================
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ================= Identity =================
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// ================= JWT Authentication =================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing!"))
        ),
        ClockSkew = TimeSpan.Zero
    };
});

// ================= Dependency Injection (DI) =================
builder.Services.AddScoped<IReviewsInterface, ReviewRepository>();
builder.Services.AddScoped<IMoviesInterface, MoviesRepository>();
builder.Services.AddScoped<ITokenService, TokenService>(); // تم حل مشكلتك السابقة هنا بوضعها في المكان الصحيح
builder.Services.AddScoped<IEmailInterface, EmailService>();
// ================= Hangfire =================
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHangfireServer();

var app = builder.Build();

//hangfire dashboard
app.UseHangfireDashboard();
// ================= Middleware Pipeline =================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Rating API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// الترتيب هنا مهم جداً: الأوث أولاً ثم الصلاحيات
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ================= Seed Database =================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        //seed roles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new[] { "admin", "user", "customer" };
        //check if not exist
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        }
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "حدث خطأ أثناء تهيئة قاعدة البيانات وبذر البيانات.");
    }
}
//create new user and assign to roles 
// ================= Seed Database & Run =================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        
        // 1. التأكد أولاً من إنشاء قاعدة البيانات لتفادي أي خطأ سياق
        context.Database.EnsureCreated(); 

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>(); // تم التعديل هنا إلى AppUser

        // 2. بذر الأدوار (Roles)
        var roles = new[] { "admin", "user", "customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 3. بذر المستخدم المسؤول (Admin User)
        string email = "jassimroles@gmail.com";
        string password = "Jassim@222";
        string adminRole = "admin"; // تأكد من مطابقة حالة الأحرف (صغيرة كما في المصفوفة أعلاه)

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            var newUser = new AppUser // تم التعديل هنا إلى AppUser
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true 
            };

            var result = await userManager.CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, adminRole);
            }
            else
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                foreach (var error in result.Errors)
                {
                    logger.LogError($"خطأ في إنشاء المستخدم: {error.Description}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "حدث خطأ أثناء تهيئة قاعدة البيانات وبذر البيانات.");
    }
}

app.Run();