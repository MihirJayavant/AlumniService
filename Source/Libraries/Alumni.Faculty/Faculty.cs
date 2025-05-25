using Core;

namespace Alumni.Faculty;


public record Faculty : IAuditableEntity
{
    public required int Id { get; init; }
    public required int FacultyId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Extension { get; init; }
    public required long MobileNo { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
