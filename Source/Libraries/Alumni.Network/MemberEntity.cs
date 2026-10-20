using Alumni.Network.Companies;
using Alumni.Network.Exams;
using Alumni.Network.FurtherStudies;

namespace Alumni.Network;

[RecordView(typeof(Member))]
public partial record MemberEntity : IAuditableEntity
{
    public int Id { get; init; }
    // External account identifier; no navigation or foreign-key constraint.
    public string? AuthUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public IList<CompanyEntity> Companies { get; init; } = [];
    public IList<ExamEntity> Exams { get; init; } = [];
    public IList<FurtherStudyEntity> FurtherStudies { get; init; } = [];
}
