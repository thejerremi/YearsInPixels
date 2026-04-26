using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using YearsInPixels.Application.Interfaces;

namespace YearsInPixels.Application.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid? UserId
        {
            get
            {
                var id = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(id, out var guid) ? guid : null;
            }
        }

        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
