using System.Xml.Serialization;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Callisto.Usps
{
    public class UspsService
    {
        private readonly string Username;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UspsService> _logger;

        public UspsService(HttpClient httpClient, IConfiguration configuration, ILogger<UspsService> logger)
        {
            httpClient.BaseAddress ??= new Uri("https://secure.shippingapis.com");
            _httpClient = httpClient;

            var username = configuration["Usps:Username"];
            if (string.IsNullOrEmpty(username)) {
                throw new ArgumentException("Usps:Username cannot be null or empty", nameof(username));
            }

            Username = username;

            _logger = logger;
        }

        public async Task<UspsAddressValidationResult> ValidateAddressAsync(Address address)
        {
            var request = new AddressValidationRequest
            {
                UserId = Username,
                Address = address
            };

            var serializer = new XmlSerializer(typeof(AddressValidationRequest));
            var stringWriter = new StringWriterWithEncoding(System.Text.Encoding.GetEncoding("ISO-8859-1"));
            serializer.Serialize(stringWriter, request);
            string xmlRequest = stringWriter.ToString();

            // Build the API request URL
            string url = $"/ShippingAPI.dll?API=Verify&XML={xmlRequest}";

            // Send the API request and get the response
            var response = await _httpClient.GetAsync(url);

            // Read the response content as an XML document
            string responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Raw Response:\n{responseContent}", responseContent);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseContent);

            // Deserialize the API response object from XML
            var responseSerializer = new XmlSerializer(typeof(AddressValidationResponse));
            using var stringReader = new StringReader(xmlDoc.InnerXml);
            var validationResponse = (AddressValidationResponse?)responseSerializer.Deserialize(stringReader);

            if (validationResponse is null)
            {
                return new(false, null, null);
            }

            if (validationResponse.Address.Error is not null)
            {
                return new(false, validationResponse.Address.Error, null);
            }

            validationResponse.Address.Transpile();
            return new(true, null, validationResponse.Address);
        }
    }
}