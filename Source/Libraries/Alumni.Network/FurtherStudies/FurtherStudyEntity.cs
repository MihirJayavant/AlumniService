namespace Alumni.Network.FurtherStudies;

public record FurtherStudyEntity : FurtherStudy
{
    public int MemberId { get; set; }
    public MemberEntity Member { get; set; } = default!;
}
