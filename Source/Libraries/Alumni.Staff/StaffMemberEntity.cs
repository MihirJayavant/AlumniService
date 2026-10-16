namespace Alumni.Staff;

[RecordView(typeof(StaffMember))]
public partial record StaffMemberEntity : IAuditableEntity
{
    public required int Id { get; init; }
    // External account identifier; no navigation or foreign-key constraint.
    public string? AuthUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
