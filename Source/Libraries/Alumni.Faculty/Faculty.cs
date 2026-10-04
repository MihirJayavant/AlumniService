namespace Alumni.Faculty;

public record Faculty : IAuditableEntity
{
    public required int Id { get; init; }
    public required Guid FacultyId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Extension { get; init; }
    public required long MobileNo { get; init; }
    // External account identifier; no navigation or foreign-key constraint.
    public string? AuthUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
