namespace Application.Contracts.Response;

public class AddressResponse
{
    public int Pincode { get; set; }
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string UserAddress { get; set; } = string.Empty;
}

