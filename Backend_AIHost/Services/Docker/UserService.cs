using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend_AIHost.Services.Interfaces;

namespace Backend_AIHost.Services.Docker
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }
    }

}
