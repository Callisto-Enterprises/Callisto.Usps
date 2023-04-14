# Callisto.Usps

## Installation

Install the package from NuGet:

```bash
dotnet add package Callisto.Usps
```

## Usage

### Basic

```json
// appsettings.json or secrets.json
{
  "Usps": {
    "Username": "XXXX"
  }
}
```

```csharp
// Program.cs
using Callisto.Usps;

// Register the service
builder.Services.AddHttpClient<UspsService>();

// ...
var app = builder.Build();

// Add Health check middleware
app.UseEndpoints(endpoints =>
{
    //...
    endpoints.MapPost("/api/validateAddress", [Authorize] async (UspsService uspsService) =>
    {
        var uspsAddress = new Address
        {
            Address1 = address.AddressLine1,
            Address2 = address.AddressLine2,
            City = address.City,
            State = address.StateProvince,
            Zip5 = address.PostalCode
        };

        var result = await uspsService.ValidateAddressAsync(uspsAddress);
        // result.Success is true if the address is valid
        // result.Error is the error message if the address is invalid
        // result.Address is the validated address if the address is valid

        return result;
    });
});
```
