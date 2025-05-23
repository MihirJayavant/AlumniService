namespace Students;

public record class Student : IAuditableEntity
{
    public required int Id { get; init; }
    public required Guid Uuid { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string MobileNo { get; init; }
    public required string Extension { get; init; }
    public required string Gender { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string Email { get; init; }
    public required string Branch { get; init; }
    public required Address CurrentAddress { get; init; }
    public required Address CorrespondenceAddress { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
}

public record Address
{
    public required string PinCode { get; init; }
    public required string Country { get; init; }
    public required string State { get; init; }
    public required string City { get; init; }
    public required string UserAddress { get; init; }
}

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
