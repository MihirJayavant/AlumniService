namespace Application.Students;

public sealed record StudentResponse
{
    public required int StudentId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }

    public required long MobileNo { get; init; }
    public required string Extension { get; init; }
    public required string Gender { get; init; }
    public required DateTime DateOfBirth { get; init; }

    public required string Branch { get; init; }
    public required AddressResponse CurrentAddress { get; init; }
    public required AddressResponse CorrespondenceAddress { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }
    public required DateTime DateCreated { get; init; }
    public required DateTime DateLastModified { get; init; }

}
