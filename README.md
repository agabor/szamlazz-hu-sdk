# .Net Core SDK for Számlázz.hu [![Build Status](https://github.com/codesharp-hu/szamlazz-hu-sdk/workflows/Test/badge.svg?branch=master)](https://github.com/BootGen/BootGen/actions) [![Coverage Status](https://coveralls.io/repos/github/codesharp-hu/szamlazz-hu-sdk/badge.svg?branch=master)](https://coveralls.io/github/codesharp-hu/szamlazz-hu-sdk?branch=master) [![NuGet](https://img.shields.io/nuget/v/szamlazz-hu-sdk.svg)](https://www.nuget.org/packages/szamlazz-hu-sdk/)

## Minimal working example

```csharp
var request = new CreateInvoiceRequest();
request.AuthenticationData.ApiKey = "<YOUR AGENT KEY>";

request.Seller.BankName = "Our Bank";
request.Seller.BankAccount = "11111111-22222222-33333333";

request.Customer.Name = "Good Friend Inc.";
request.Customer.CustomerAddress.PostalCode = "1132";
request.Customer.CustomerAddress.City = "Budapest";
request.Customer.CustomerAddress.StreetAddress = "Tárnoki út 23.";
request.Customer.TaxNumber = "12345678-1-42";

request.Items = new List<InvoiceItem> {
    new InvoiceItem {
        Name = "Something we sell",
        Quantity = 1,
        UnitOfQuantity = "piece",
        UnitPrice = 1000,
        VatRate = "EU",
        NetPrice = 1000,
        VatAmount = 0,
        GrossAmount = 1000
    }
};

var api = new SzamlazzHuApi();
var response = await api.CreateInvoice(request);

//Optionally write invoice PDF to the disk
File.WriteAllBytes("invoice.pdf", response.InvoicePdf);
```
