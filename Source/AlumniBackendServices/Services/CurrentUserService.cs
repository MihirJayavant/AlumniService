using System.Security.Claims;
using Application.Common.Interfaces;

namespace AlumniBackendServices.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    => this.httpContextAccessor = httpContextAccessor;

    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
