namespace Core.Contracts.Response
{
    public class AddressResponse
    {
       public int Pincode { get; set; }
       public string Country { get; set; }
       public string State { get; set; }
       public string City { get; set; }
       public string UserAddress { get; set; }
    }
}
