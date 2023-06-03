using System.Text;
using AlumniBackendServices.Services;
using Application.Common.Interfaces;
using Database;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AlumniBackendServices.ExtensionService;

public static class AuthExtension
{
    public static void AddAuth(this IServiceCollection services) => services.AddSingleton<ICurrentUserService, CurrentUserService>();

    public static void UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
