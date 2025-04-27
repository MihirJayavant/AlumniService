namespace Application.Students;

public sealed record AddressResponse
{
    public required int Pincode { get; init; }
    public required string Country { get; init; }
    public required string State { get; init; }
    public required string City { get; init; }
    public required string UserAddress { get; init; }
}

