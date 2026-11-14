using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FinalProjectWebAPI.Data;
using FinalProjectWebAPI.Services.Implementations;
using FinalProjectWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//1. DataBase
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//2. Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWhatsAppService, MockWhatsAppService>();
builder.Services.AddScoped<IAdService, AdService>();
builder.Services.AddScoped<IShowroomService, ShowroomService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 1.  ⁄—Ì› ÿ—Ìﬁ… «·„’«œﬁ… (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiJBbGlhIiwicm9sZSI6Ik5vcm1hbFVzZXIiLCJQaG9uZSI6Ijc4MDc5NzgxOCIsIm5iZiI6MTc5Mzg2MDA0NSwiZXhwIjoxNzk0NDY0ODQ1LCJpYXQiOjE3OTM4NjAwNDUsImlzcyI6IkZpbmFsUHJvamVjdFdlYkFQSSIsImF1ZCI6IkZpbmFMUHJvamVjdFdlYkFQSSJ9.PU3yG5h6rlo0QOiO9wvQB_mk8K5-9ZTqXnCLauIzJDs"
    });

    // 2.  ÿ»Ìﬁ «·„’«œﬁ… ⁄·Ï Ã„Ì⁄ «·‹ Endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});// 3. ≈⁄œ«œ «·„’«œﬁ… (JWT Authentication)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// 4.  ›⁄Ì· «·„’«œﬁ… Ê«· ›ÊÌ÷ (ÌÃ» √‰ ÌﬂÊ‰« ﬁ»· MapControllers)
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
