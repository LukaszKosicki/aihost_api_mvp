using System.Threading.Tasks;
using Backend_AIHost.Models;
using Backend_AIHost.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    public AuthController(UserManager<AppUser> userManager,
                          IConfiguration configuration, IAuthService authService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _authService = authService;
        _roleManager = roleManager;
    }

    [HttpGet("check")]
    [Authorize]
    public async Task<IActionResult> Check()
    {

        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { loggedIn = true, email = user.Email, role = roles.FirstOrDefault() });
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
}

public record ChangePasswordDto(string CurrentPassword, string NewPassword);
public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
