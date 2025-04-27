namespace Core.ValueObjects;

public record Address(
    int Pincode,
    string Country,
    string State,
    string City,
    string UserAddress
);
