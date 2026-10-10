using Alumni.Auth;
using Alumni.Auth.Invitations;
using Infrastructure.Auth;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public class InvitationTokenTests
{
    [Fact]
    public void Create_WhenCalled_ProducesUrlSafe256BitToken()
    {
        var token = InvitationToken.Create();

        Assert.Equal(43, token.Length);
        Assert.Matches("^[A-Za-z0-9_-]{43}$", token);
        Assert.Equal(32, WebEncoders.Base64UrlDecode(token).Length);
        Assert.True(InvitationToken.TryHash(token, out var hash));
        Assert.Equal(InvitationToken.Hash(token), hash);
    }

    [Fact]
    public void Create_WhenCalledRepeatedly_ProducesDistinctTokens()
    {
        var tokens = Enumerable.Range(0, 16).Select(_ => InvitationToken.Create()).ToArray();

        Assert.Equal(tokens.Length, tokens.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Hash_WhenTokenIsKnown_ReturnsExpectedSha256Digest()
    {
        var token = new string('A', 43);

        var hash = InvitationToken.Hash(token);

        Assert.Equal("0F007385B6F9D4B7EEB2748605AFE1A984A0A3BFA3F014D09E2A784CE9E5CD1A", hash);
        Assert.Matches("^[0-9A-F]{64}$", hash);
        Assert.NotEqual(token, hash);
        Assert.Equal(hash, InvitationToken.Hash(token));
        Assert.NotEqual(hash, InvitationToken.Hash(new string('A', 42) + "B"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA+")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA ")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAé")]
    public void TryHash_WhenTokenIsMalformed_RejectsWithoutProducingHash(string? token)
    {
        Assert.False(InvitationToken.TryHash(token, out var hash));
        Assert.Equal(string.Empty, hash);
    }

    [Fact]
    public void AcceptInvitation_WhenFormatted_DoesNotExposeCredentials()
    {
        var request = new AcceptInvitation("secret-invitation-token", "secret-new-password");

        var formatted = request.ToString();

        Assert.DoesNotContain(request.Token, formatted, StringComparison.Ordinal);
        Assert.DoesNotContain(request.Password, formatted, StringComparison.Ordinal);
    }

    [Fact]
    public void InvitationIssued_WhenFormatted_DoesNotExposeToken()
    {
        var invitation = new InvitationIssued(Guid.NewGuid(), "account-123", "person@example.com",
            AuthRoles.Student, "secret-invitation-token", DateTimeOffset.UnixEpoch.AddDays(2));

        Assert.DoesNotContain(invitation.Token, invitation.ToString(), StringComparison.Ordinal);
    }
}
