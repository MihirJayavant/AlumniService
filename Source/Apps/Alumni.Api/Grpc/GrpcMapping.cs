namespace Alumni.Api.Grpc;

internal static class GrpcMapping
{
    public static Contracts.PaginationMetadata Pagination<T>(PaginatedList<T> page)
        => new()
        {
            TotalCount = page.TotalCount,
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalPages = page.TotalPages,
            HasPreviousPage = page.HasPreviousPage,
            HasNextPage = page.HasNextPage
        };

    public static Contracts.AddressMessage Address(Address address)
        => new()
        {
            PinCode = address.PinCode,
            Country = address.Country,
            State = address.State,
            City = address.City,
            UserAddress = address.UserAddress
        };
}
