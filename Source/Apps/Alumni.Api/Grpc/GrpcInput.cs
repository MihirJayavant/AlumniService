using System.Globalization;
using Grpc.Core;

namespace Alumni.Api.Grpc;

internal static class GrpcInput
{
    public static Guid Guid(string value, string fieldName)
    {
        if (System.Guid.TryParse(value, out var result))
        {
            return result;
        }

        throw Invalid(fieldName, "must be a GUID");
    }

    public static Guid OptionalGuid(string value, string fieldName)
        => string.IsNullOrEmpty(value) ? System.Guid.Empty : Guid(value, fieldName);

    public static DateOnly Date(string value, string fieldName)
    {
        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var result))
        {
            return result;
        }

        throw Invalid(fieldName, "must be a calendar date in yyyy-MM-dd format");
    }

    public static Address Address(Contracts.AddressMessage? value, string fieldName)
    {
        if (value is null)
        {
            throw Invalid(fieldName, "is required");
        }

        return new Address
        {
            PinCode = value.PinCode,
            Country = value.Country,
            State = value.State,
            City = value.City,
            UserAddress = value.UserAddress
        };
    }

    private static RpcException Invalid(string fieldName, string detail)
        => new(new Status(StatusCode.InvalidArgument, $"{fieldName} {detail}."));
}
