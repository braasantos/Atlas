using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using Atlas.Data;
using Atlas.Models;
using Atlas.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Controllers;

public sealed class RegisterReq
{
    [Required, MaxLength(255)] public string Username { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string Name { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(255)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string Password { get; set; } = string.Empty;
}

public sealed class LoginReq
{
    [Required, EmailAddress, MaxLength(255)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string Password { get; set; } = string.Empty;
}

public sealed class RefreshReq
{
    [Required] public string RefreshToken { get; set; } = string.Empty;
}

[ApiController]
[Route("auth")]
public sealed class AtlasAuth(AtlasDbContext db, IConfiguration configuration) : ControllerBase
{
    private readonly AtlasDbContext _db = db;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterReq request)
    {
        if (!Utils.IsInputValid(request.Name, request.Username, request.Email, request.Password))
            return BadRequest("Invalid registration details");

        var email = Utils.NormalizeEmail(request.Email);
        var username = request.Username.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email || u.Username == username))
            return Conflict("Email or username already in use");

        var user = new User
        {
            Name = request.Name.Trim(),
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Created("/auth/login", new { user.Id, user.Username, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginReq request)
    {
        var email = Utils.NormalizeEmail(request.Email);
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password");

        return await IssueTokens(user);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshReq request)
    {
        if (!HasSigningKey()) return MissingSigningKey();
        var tokenHash = Utils.HashRefreshToken(request.RefreshToken);
        var stored = await _db.RefreshTokens.Include(t => t.User)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash);
        if (stored is null || stored.ExpiresAt <= DateTime.UtcNow)
            return Unauthorized("Refresh token invalid or expired");

        _db.RefreshTokens.Remove(stored);
        await _db.SaveChangesAsync();
        return await IssueTokens(stored.User);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshReq request)
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var userId)) return Unauthorized();
        var hash = Utils.HashRefreshToken(request.RefreshToken);
        var stored = await _db.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == hash && t.UserId == userId);
        if (stored is null) return Unauthorized("Refresh token invalid");
        _db.RefreshTokens.Remove(stored);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<IActionResult> IssueTokens(User user)
    {
        var key = _configuration["Jwt:Key"];
        if (!HasSigningKey()) return MissingSigningKey();

        var issuer = _configuration["Jwt:Issuer"] ?? "Atlas";
        var audience = _configuration["Jwt:Audience"] ?? "Atlas";
        var plainToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        _db.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = Utils.HashRefreshToken(plainToken),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        await _db.SaveChangesAsync();

        var accessToken = Utils.GenerateJwt(user, key!, issuer, audience);
        return Ok(new { accessToken, refreshToken = plainToken });
    }

    private bool HasSigningKey() =>
        !string.IsNullOrWhiteSpace(_configuration["Jwt:Key"]) &&
        System.Text.Encoding.UTF8.GetByteCount(_configuration["Jwt:Key"]!) >= 32;

    private ObjectResult MissingSigningKey() =>
        Problem("Configure Jwt:Key with at least 32 bytes before using authentication.", statusCode: 503);
}
