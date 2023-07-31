using System.Xml.Serialization;

namespace Callisto.Usps
{
    // The root element of the USPS Address Validation API request
    [XmlRoot("AddressValidateRequest")]
    public sealed class AddressValidationRequest
    {
        [XmlAttribute("USERID")]
        public string UserId { get; set; } = null!;

        [XmlElement("Address")]
        public Address Address { get; set; } = null!;
    }

    // The Address element of the USPS Address Validation API request
    public sealed class Address
    {
        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlElement("Address1")]
        public string? Address1 { get; set; }

        [XmlElement("Address2")]
        public string? Address2 { get; set; } = string.Empty;

        [XmlElement("City")]
        public string? City { get; set; }

        [XmlElement("State")]
        public string? State { get; set; }

        [XmlElement("Zip5")]
        public string? Zip5 { get; set; }

        [XmlElement("Zip4")]
        public string? Zip4 { get; set; } = string.Empty;

        public Error? Error { get; set; }

        // USPS API returns Address2 in Address1. This method will swap them.
        public void Transpile()
        {
            if (!string.IsNullOrEmpty(Address2))
            {
                var line1 = Address2;
                var line2 = Address1;

                Address1 = line1;
                Address2 = line2;
            }
        }
    }

    public class Error
    {
        [XmlElement("Number")]
        public string? Number { get; set; }

        [XmlElement("Source")]
        public string? Source { get; set; }

        [XmlElement("Description")]
        public string? Description { get; set; }

        [XmlElement("HelpFile")]
        public string? HelpFile { get; set; }

        [XmlElement("HelpContext")]
        public string? HelpContext { get; set; }
    }

    // The root element of the USPS Address Validation API response
    [XmlRoot("AddressValidateResponse")]
    public sealed class AddressValidationResponse
    {
        [XmlElement("Address")]
        public Address Address { get; set; } = null!;
    }

    // The root element of the USPS Address Validation API response for Errors
    [XmlRoot("Error")]
    public sealed class AddressValidationErrorResponse : Error
    {
    }

    public sealed class UspsAddressValidationResult
    {
        public UspsAddressValidationResult(bool success, Error? error, Address? address)
        {
            Success = success;
            Error = error;
            Address = address;
        }

        public bool Success { get; set; }
        public Error? Error { get; set; }

        public Address? Address { get; set; }
    }
}