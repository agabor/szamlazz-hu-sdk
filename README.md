# .NET SDK for Számlázz.hu [![Test](https://github.com/agabor/szamlazz-hu-sdk/actions/workflows/test.yml/badge.svg)](https://github.com/agabor/szamlazz-hu-sdk/actions/workflows/test.yml) [![Coverage Status](https://coveralls.io/repos/github/codesharp-hu/szamlazz-hu-sdk/badge.svg?branch=master)](https://coveralls.io/github/codesharp-hu/szamlazz-hu-sdk?branch=master) [![NuGet](https://img.shields.io/nuget/v/szamlazz-hu-sdk.svg)](https://www.nuget.org/packages/szamlazz-hu-sdk/)

## Compatibility

This SDK supports .NET 9.0 and later versions.

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

// Optional: Set reference to a pro forma invoice number
request.Header.ProFormaInvoiceNumber = "D-2025-001";

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

## Query taxpayers

This is used to query the validity of a VAT number and to get additional information about the taxpayer. The data is from the Online Invoice Platform of NAV, the Hungarian National Tax and Customs Administration.

### Request building

The API endpoint can be used through the `SzamlazzHuApi.QueryTaxpayer` method, which requires a `QueryTaxpayerRequest` object as parameter. The request object contains authentication data, and a core tax nuber.

```csharp
var request = new QueryTaxpayerRequest();
request.AuthenticationData.ApiKey = "<YOUR AGENT KEY>";
request.TaxpayerId = "<CORE TAX NUMBER (FIRST 8 DIGIT OF THE VAT NUMBER)>"

var api = new SzamlazzHuApi();
var response = await api.QueryTaxpayer(request);
```

### Response

The response object contains information about the technical result of the query and optionally details about the taxpayer. Additional data about the holder of the VAT number is only presented if:
1) The request was successful (`response.Success == true`)
2) There is a valid taxpayer on the given core tax number. (`response.TaxpayerValid == true`)

In other cases `response.Taxpayer` can be `null` 

```csharp
public class QueryTaxpayerResponse
{
    // True, if the query was successful and there were no technical error in communication
    public bool Success { get; set; }
    // Contains error code about request. It is -1, if the query was successful
    public int ErrorCode { get; set; }
    // Contains error message. It is null if query was successful
    public string ErrorMessage { get; set; }
    // Holds information about the validity of the VAT number
    public bool TaxpayerValid { get; set; }
    // Contains everything about the holder of the queried VAT number. It can be null in case the TaxpayerValid is false
    public Taxpayer Taxpayer { get; set; }       
}
```

#### Taxpayer entity

This entity holds all of the data, which can be found in the NAV database. It has required and optional properties, which is based on the NAV API definition. For further information please check the API definition of [NAV Online Invoicing](https://github.com/nav-gov-hu/Online-Invoice). 

```csharp
public class Taxpayer
{
    // Contains the legal name of the taxpayer
    public string TaxpayerName { get; set; }
    // Contains the short name of the taxpayer, which can be null               
    public string TaxpayerShortName { get; set; }
    // Contains tax number of the taxpayer
    public TaxNumber TaxNumberDetail { get; set; }
    // Contains tax number of the VAT group, if taxpayer has group membership
    public TaxNumber VatGroupMembership { get; set; }
    // Contains the incorporation type of the taxpayer
    public string Incorporation { get; set; }
    // Contains a list of the taxpayer addresses. This can be empty list   
    public List<TaxpayerAddressItem> AddressList { get; set; }
}
```

#### TaxNumber entity

This entity contains a tax number. Based on the NAV API definition, the `TaxpayerId` is the only mandatory part of this entity. The other properties are just optional, don't take part of the identification.

```csharp
public class TaxNumber
{
    // This contains the core tax number (XXXXXXXX)-X-XX
    public string TaxpayerId { get; set; }
    // This contains the VAT code, it can be null if it is not presented (eg. group VAT number) XXXXXXXX-(X)-XX
    public string VatCode { get; set; }
    // This contains the country code, it can be null if it is not presented (eg. group VAT number) XXXXXXXX-X-(XX)
    public string CountryCode { get; set; }
}
```

#### TaxpayerAddressItem entity

This entity contains an address and also holds information about the type of the address. Address types can be found in NAV documentation.

```csharp
public class TaxpayerAddressItem
{
    // This holds information about the type of the address (eg. HQ, SITE, etc.). See NAV documentation about the possible codes.
    public string AddressType { get; set; }
    // This contains the address    
    public Address Address { get; set; }
}
```

## Pull Requests are Welcome!

If you are missing something please open an issue, or send a pull request! Every contribution is welcome :)
