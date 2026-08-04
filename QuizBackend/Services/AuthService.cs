using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuizBackend.Data;
using QuizBackend.DTOs.Auth;
using QuizBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuizBackend.Services;

public class AuthService
{
    private readonly QuizDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(QuizDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var fullName = dto.FullName?.Trim() ?? string.Empty;
        var email = dto.Email?.Trim() ?? string.Empty;
        var password = dto.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(fullName))
            throw new Exception("Full name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new Exception("Email is required.");

        if (password.Length < 4)
            throw new Exception("Password must be at least 4 characters.");

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (existingUser != null)
        {
            throw new Exception("Email already exists.");
        }

        var passwordHash = HashPassword(password);

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new RegisterResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        var passwordHash = HashPassword(dto.Password);

        if (user.PasswordHash != passwordHash)
        {
            throw new Exception("Invalid email or password.");
        }

        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = token
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();

        var bytes = Encoding.UTF8.GetBytes(password);

        var hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}
