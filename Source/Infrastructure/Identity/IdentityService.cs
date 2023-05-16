using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory;

    public IdentityService(
        UserManager<IdentityUser> userManager,
        IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory)
    {
        this.userManager = userManager;
        this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<bool> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await userManager.CreateAsync(user, password);

        return result is not null;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await userClaimsPrincipalFactory.CreateAsync(user);

        // var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await userManager.Users.SingleAsync(u => u.Id == userId);

        var result = await userManager.DeleteAsync(user);

        return result is not null;
    }

}
