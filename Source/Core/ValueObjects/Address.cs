
using System;
using System.Diagnostics.CodeAnalysis;

namespace Core.ValueObjects
{
    public class Address : IEquatable<Address>
    {
        public int Pincode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string UserAddress { get; set; }

        public override bool Equals(object obj)
            => obj switch
            {
                null => false,
                Address right=> this == right,
                _ => false
            };

        public bool Equals(Address other)
                => other switch {
                    null => false,
                    _ => this == other
                };

        public override int GetHashCode()
            => Pincode ^ Country.GetHashCode() ^ State.GetHashCode()
                ^ City.GetHashCode() ^ UserAddress.GetHashCode();

        public override string ToString()
            => $"{UserAddress}, {City}, {State}, {Country}, {Pincode}";

        public static bool operator ==(Address left, Address right)
            => left.Pincode == right.Pincode
            && left.Country.Equals(right.Country, StringComparison.CurrentCultureIgnoreCase)
            && left.State.Equals(right.State, StringComparison.CurrentCultureIgnoreCase)
            && left.City.Equals(right.Country, StringComparison.CurrentCultureIgnoreCase)
            && left.UserAddress.Equals(right.UserAddress, StringComparison.CurrentCultureIgnoreCase);

        public static bool operator !=(Address left, Address right)
            => !(left == right);
    }
}
