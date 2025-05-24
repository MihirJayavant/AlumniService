namespace Application.Faculties;

public sealed record FacultyResponse
{
    public required int FacultyId { get; init; }
    public required string Email { get; init; }
    public required DateTime DateCreated { get; init; }
    public required bool Admin { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required long MobileNo { get; init; }
    public required string Extension { get; init; }
}

