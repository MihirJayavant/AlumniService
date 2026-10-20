namespace Alumni.Network.Companies;

public record CompanyEntity : Company
{
    public required int MemberId { get; init; }
    public required MemberEntity Member { get; init; }
}
