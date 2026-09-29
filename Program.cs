using System.Text;
using Atlas.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AtlasDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Atlas") ?? "Data Source=atlas.db"));

var jwtKey = builder.Configuration["Jwt:Key"] ?? Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Atlas",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Atlas",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AtlasDbContext>();
    db.Database.EnsureCreated();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Atlas API is running");

app.Run();
