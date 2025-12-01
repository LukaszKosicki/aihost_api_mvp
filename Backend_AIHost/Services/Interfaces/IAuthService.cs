using Backend_AIHost.DTOs.User;
using Backend_AIHost.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend_AIHost.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(string email, string password);
        Task<LoginResponse> LoginAsync(string email, string password);
        string GenerateJwtToken(AppUser user);
    }
}
