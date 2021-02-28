using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AlumniBackendServices.Options;
using Core.Contracts.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AlumniBackendServices.Services
{
    public interface IIdentityService
    {
        Task<Response<StudentRegisteredResponse>> RegisterStudent(string email, string password);
        Task<Response<StudentLoginResponse>> StudentLogin(string email, string password);
    }

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> manager;
        private readonly IConfiguration configuration;

        public IdentityService(UserManager<IdentityUser> manager, IConfiguration configuration)
        => (this.manager, this.configuration) = (manager, configuration);

        public async Task<Response<StudentRegisteredResponse>> RegisterStudent(string email, string password)
        {
            var existingUser = await manager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return new()
                {
                    Error = new("Student already registered"),
                    Status = ResponseStatus.BadRequest
                };
            }

            IdentityUser user = new()
            {
                UserName = email,
                Email = email
            };

            var createdUser = await manager.CreateAsync(user, password);

            if (createdUser.Succeeded == false)
            {
                return new()
                {
                    Error = new(createdUser.Errors.ToArray()[0].Description),
                    Status = ResponseStatus.BadRequest
                };
            }


            var (tokenHandler, token) = GenerateToken(user);

            return new()
            {
                Status = ResponseStatus.Success,
                Result = new()
                {
                    Description = "Student Account Created",
                    Email = user.Email,
                    Token = tokenHandler.WriteToken(token)
                }
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
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            return (tokenHandler, token);
        }

        public async Task<Response<StudentLoginResponse>> StudentLogin(string email, string password)
        {
            var user = await manager.FindByEmailAsync(email);

            if (user is null)
            {
                return new()
                {
                    Error = new("Email / Passsword Invalid"),
                    Status = ResponseStatus.BadRequest
                };
            }

            var userHasValidPassword = await manager.CheckPasswordAsync(user, password);

            if (userHasValidPassword == false)
            {
                return new()
                {
                    Error = new("Email / Passsword Invalid"),
                    Status = ResponseStatus.BadRequest
                };
            }


            var (tokenHandler, token) = GenerateToken(user);

            return new()
            {
                Status = ResponseStatus.Success,
                Result = new()
                {
                    Email = user.Email,
                    Token = tokenHandler.WriteToken(token)
                }
            };

        }
    }
}
