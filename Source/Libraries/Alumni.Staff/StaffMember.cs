namespace Alumni.Staff;

public record StaffMember
{
    public required Guid FacultyId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Extension { get; init; }
    public required long MobileNo { get; init; }
}
