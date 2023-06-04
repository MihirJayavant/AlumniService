using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OneOf;

namespace Infrastructure.Identity;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
}
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> manager;
    private readonly ISettingService settingService;
    private readonly RoleManager<IdentityRole> roleManager;

    public IdentityService(UserManager<ApplicationUser> manager, RoleManager<IdentityRole> roleManager, ISettingService settingService)
    => (this.manager, this.roleManager, this.settingService) = (manager, roleManager, settingService);

    public async Task<OneOf<IdentityResponse, ErrorType>> RegisterStudent(string email, string password)
    {
        var existingUser = await manager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return new ErrorType(ResponseStatus.BadRequest, "Student already registered");
        }

        ApplicationUser user = new()
        {
            UserName = email,
            Email = email
        };

        var createdUser = await manager.CreateAsync(user, password);

        if (createdUser.Succeeded == false)
        {
            return new ErrorType(ResponseStatus.BadRequest, createdUser.Errors.ToArray()[0].Description);
        }

        var exists = await roleManager.RoleExistsAsync("Student");

        if (exists is false)
        {
            await AddCustomRole("Student");
        }

        await manager.AddToRoleAsync(user, "Student");

        var roles = await manager.GetRolesAsync(user);

        var (tokenHandler, token) = GenerateToken(user, roles);

        return new IdentityResponse()
        {
            Email = user.Email,
            Token = tokenHandler.WriteToken(token)
        };
    }

    public async Task<OneOf<IdentityResponse, ErrorType>> StudentLogin(string email, string password)
    {
        var user = await manager.FindByEmailAsync(email);

        if (user is null)
        {
            return new ErrorType(ResponseStatus.BadRequest, "Email / Password Invalid");
        }

        var userHasValidPassword = await manager.CheckPasswordAsync(user, password);

        if (userHasValidPassword == false)
        {
            return new ErrorType(ResponseStatus.BadRequest, "Email / Password Invalid");
        }

        var roles = await manager.GetRolesAsync(user);

        var (tokenHandler, token) = GenerateToken(user, roles);

        return new IdentityResponse()
        {
            Email = user.Email ?? "",
            Token = tokenHandler.WriteToken(token)
        };
    }

    private (JwtSecurityTokenHandler, SecurityToken) GenerateToken(ApplicationUser user, IList<string> roles)
    {
        JwtSecurityTokenHandler tokenHandler = new();

        JwtOptions options = new()
        {
            Secret = settingService.AuthSetting.Secret
        };

        var key = Encoding.ASCII.GetBytes(options.Secret);

        var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Aud, settingService.AuthSetting.ValidAudience),
                new Claim(JwtRegisteredClaimNames.Iss, settingService.AuthSetting.ValidIssuer),
                new Claim("id", user.Id)
            };

        foreach (var role in roles)
        {
            claims.Add(new Claim("roles", role));
        }


        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(descriptor);
        return (tokenHandler, token);
    }

    public async Task<OneOf<RoleResponse, ErrorType>> AddCustomRole(string role)
    {
        var result = await roleManager.CreateAsync(new(role));
        if (result.Succeeded is false)
        {
            return new ErrorType(ResponseStatus.BadRequest, result.Errors.ToArray()[0].Description);
        }

        return new RoleResponse();
    }
}
