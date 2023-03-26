using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlumniBackendServices.Options;
using Application.Common.Models;
using Application.Students;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OneOf;

namespace AlumniBackendServices.Services
{
    public interface IIdentityService
    {
        Task<OneOf<StudentRegisteredResponse, ErrorType>> RegisterStudent(string email, string password);
        Task<OneOf<StudentLoginResponse, ErrorType>> StudentLogin(string email, string password);
    }

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> manager;
        private readonly IConfiguration configuration;

        public IdentityService(UserManager<IdentityUser> manager, IConfiguration configuration)
        => (this.manager, this.configuration) = (manager, configuration);

        public async Task<OneOf<StudentRegisteredResponse, ErrorType>> RegisterStudent(string email, string password)
        {
            var existingUser = await manager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return new ErrorType(ResponseStatus.BadRequest, "Student already registered");
            }

            IdentityUser user = new()
            {
                UserName = email,
                Email = email
            };

            var createdUser = await manager.CreateAsync(user, password);

            if (createdUser.Succeeded == false)
            {
                return new ErrorType(ResponseStatus.BadRequest, createdUser.Errors.ToArray()[0].Description);
            }


            var (tokenHandler, token) = GenerateToken(user);

            return new StudentRegisteredResponse()
            {
                Description = "Student Account Created",
                Email = user.Email,
                Token = tokenHandler.WriteToken(token)
            };
        }

        private (JwtSecurityTokenHandler, SecurityToken) GenerateToken(IdentityUser user)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            JwtOptions options = new();
            configuration.Bind(nameof(JwtOptions), options);

            var key = Encoding.ASCII.GetBytes(options.Secret);

            SecurityTokenDescriptor descriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            return (tokenHandler, token);
        }

        public async Task<OneOf<StudentLoginResponse, ErrorType>> StudentLogin(string email, string password)
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


            var (tokenHandler, token) = GenerateToken(user);

            return new StudentLoginResponse()
            {
                Email = user.Email ?? "",
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
