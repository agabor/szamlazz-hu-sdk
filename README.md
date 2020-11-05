# .Net Core SDK for Számlázz.hu

## Minimal working example

```csharp
var request = new CreateInvoiceRequest();
request.Settings.ApiKey = "<YOUR AGENT KEY>";

request.Seller.BankName = "Our Bank";
request.Seller.BankAccount = "11111111-22222222-33333333";

request.Customer.CustomerContact.Name = "Good Friend Inc.";
request.Customer.CustomerContact.PostalCode = "1132";
request.Customer.CustomerContact.City = "Budapest";
request.Customer.CustomerContact.Address = "Tárnoki út 23.";
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
File.WriteAllBytes("invoice.pdf", response.pdf);
```
