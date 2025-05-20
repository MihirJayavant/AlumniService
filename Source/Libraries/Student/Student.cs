using Domain;

namespace Student;

public record class Student : IEntity<string>
{
    public required string Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required long MobileNo { get; init; }
    public required string Extension { get; init; }
    public required string Gender { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string Email { get; init; }
    public required string Branch { get; init; }
    public required Address CurrentAddress { get; init; }
    public required Address CorrespondenceAddress { get; init; }
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

public static class Branch
{
    public static readonly string IT = "IT";
    public static readonly string CS = "CS";
    public static readonly string EXTC = "EXTC";
    public static readonly string ELEX = "ELEX";
}

public static class Gender
{
    public static readonly string Male = "Male";
    public static readonly string Female = "Female";
}
