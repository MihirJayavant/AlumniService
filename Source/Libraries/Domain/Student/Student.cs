using Domain.Common;

namespace Domain.Student;

public record class Student : IEntity<string>
{
    public required string Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required long MobileNo { get; init; }
    public required string Extension { get; init; }
    public required Gender Gender { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string Email { get; init; }
    public required Branch Branch { get; init; }
    public required Address CurrentAddress { get; init; }
    public required Address CorrespondanceAddress { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }
}

public record Address(
    int Pincode,
    string Country,
    string State,
    string City,
    string UserAddress
);

public enum Branch
{
    IT = 1,
    CS = 2,
    EXTC = 3,
    ELEX = 4
}

public enum Gender
{
    Male = 1,
    Female = 2
}
