using Backend_AIHost.Models;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    public AuthController(UserManager<AppUser> userManager,
                          IConfiguration configuration, IAuthService authService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _authService = authService;
    }

    [HttpGet("check")]
    [Authorize]
    public IActionResult Check()
    {
        return Ok(new { loggedIn = true, user = User.Identity?.Name });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { variant = "error", title = "Error", description = result.Errors.First().Description }); 
        }

        return Ok(new { variant = "success",title = "Congratulations", description = "Password changed successfully" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto.Email, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok("Użytkownik zarejestrowany!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token == null) return Unauthorized();
        return Ok(token);
    }


    private string GenerateJwtToken(IdentityUser user)
    {
        var claims = new[]
 {
    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
    new Claim(ClaimTypes.NameIdentifier, user.Id), // teraz na pewno trafi do HttpContext.User
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record ChangePasswordDto(string CurrentPassword, string NewPassword);
public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
