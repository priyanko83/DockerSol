using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.ValueObjects
{
    public class Address : ValueObject<Address>
    {
        readonly string Street;
        readonly string City;
        readonly string Country;
        readonly string ZipCode;

        public Address(string street, string city, string country, string zipCode)
        {
            this.Street = street;
            this.City = city;
            this.Country = country;
            this.ZipCode = zipCode;

        }
    }
}
