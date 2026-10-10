using Alumni.Auth.Invitations;
using Core;
using OneOf;

namespace Infrastructure.Auth;

public interface IInvitationProvisioningService
{
    public Task<OneOf<InvitationIssued, ErrorType>> InviteStudentAsync(string invitedByUserId, AddStudent profile, CancellationToken cancellationToken = default);
    public Task<OneOf<InvitationIssued, ErrorType>> InviteFacultyAsync(string invitedByUserId, AddFaculty profile, string role, CancellationToken cancellationToken = default);
}
