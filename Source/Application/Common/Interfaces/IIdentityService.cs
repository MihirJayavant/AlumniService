using Application.Identity;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<OneOf<IdentityResponse, ErrorType>> RegisterStudent(string email, string password);
    Task<OneOf<IdentityResponse, ErrorType>> StudentLogin(string email, string password);
}
