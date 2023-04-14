using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Xunit.Abstractions;

namespace Callisto.Usps.Tests
{
    public class UnitTests
    {
        private readonly UspsService _uspsService;
        private readonly ITestOutputHelper _output;
        private readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public UnitTests(ITestOutputHelper output)
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<UnitTests>().Build();
            var httpClient = new HttpClient();

            _uspsService = new UspsService(httpClient, configuration);
            _output = output;
        }

        [Fact]
        public async Task TestValid()
        {
            var address = new Address
            {
                Address1 = "543 W WestMoreland St",
                Address2 = "Suite 200",
                City = "Philadelphia",
                State = "PA",
                Zip5 = "19140"
            };

            var result = await _uspsService.ValidateAddressAsync(address);
            _output.WriteLine(JsonSerializer.Serialize(result, Options));
            Assert.True(result.Success);
            Assert.NotNull(result.Address);
        }

        [Fact]
        public async Task TestAutocorrect()
        {
            var address = new Address
            {
                Address1 = "220 N Ann Ave",
                City = "Dover",
                State = "DE",
                Zip5 = "18000"
            };

            var result = await _uspsService.ValidateAddressAsync(address);
            _output.WriteLine(JsonSerializer.Serialize(result, Options));
            Assert.True(result.Success);
            Assert.NotNull(result.Address);
            Assert.Equal("19904", result.Address.Zip5);
        }

        [Fact]
        public async Task TestInvalid()
        {
            var address = new Address
            {
                Address1 = "10 Main Street Way",
                Address2 = string.Empty,
                City = "Dogsville",
                State = "PA",
                Zip5 = "XXXX",
                Zip4 = string.Empty
            };

            var result = await _uspsService.ValidateAddressAsync(address);
            _output.WriteLine(JsonSerializer.Serialize(result, Options));
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Null(result.Address);
        }
    }
}