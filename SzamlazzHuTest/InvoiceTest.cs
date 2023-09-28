using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.XmlDiffPatch;
using SzamlazzHu;

namespace SzamlazzHuTest;

[TestClass]
public class InvoiceTest
{
    [TestMethod]
    public void RenderXML()
    {
        var readerSettings = new XmlReaderSettings();
        readerSettings.Schemas.Add("http://www.szamlazz.hu/xmlszamla", "xmlszamla.xsd");
        readerSettings.ValidationType = ValidationType.Schema;
        readerSettings.ValidationEventHandler += (s, e) => Assert.Fail(e.Message);

        var request = CreateSampleRequest();

        using (var xmlStream = XMLRenderer.RenderRequest(request))
        {
            var reader = XmlReader.Create(xmlStream, readerSettings);
            var doc = new XmlDocument();
            doc.Load(reader);
            var originalDoc = new XmlDocument();
            originalDoc.Load("testCreateInvoiceRequest.xml");
            var xmldiff = new XmlDiff(XmlDiffOptions.IgnoreChildOrder | 
                                                XmlDiffOptions.IgnoreNamespaces |
                                                XmlDiffOptions.IgnoreComments | 
                                                XmlDiffOptions.IgnorePrefixes);
            using (var output = new MemoryStream())
            {
                var diffgramWriter = XmlWriter.Create(output);
                bool bIdentical = xmldiff.Compare(originalDoc.DocumentElement, doc.DocumentElement, diffgramWriter);
                output.Position = 0;
                var diff = new StreamReader(output).ReadToEnd();
                Assert.IsTrue(bIdentical, diff);
            }
        }
    }

    [TestMethod]
    public void ResponseParseTest()
    {
        var doc = new XmlDocument();
        doc.Load("testGetInvoiceResponse.xml");
        var response = XmlParser.ParseGetInvoiceResponse(doc);
        Assert.AreEqual(new DateTime(2020, 11, 10), response.InvoiceHeader.DueDate);
        Assert.AreEqual("Kovács & Társa Bt.", response.Customer.Name);
        Assert.AreEqual(new DateTime(2020, 09, 22) , response.PaymentItems[0].Date);
        Assert.AreEqual("transfer", response.PaymentItems[0].Title);
        Assert.AreEqual(15, response.PaymentItems[0].Amount);
        Assert.AreEqual("comment", response.PaymentItems[0].Comment);
        Assert.AreEqual("111167564355767895454564", response.PaymentItems[0].BankAccountNumber);
        Assert.AreEqual(27, response.Summaries.VatRateSums.VatRate);
        Assert.AreEqual(30000, response.Summaries.VatRateSums.Net);
        Assert.AreEqual(8100, response.Summaries.VatRateSums.Vat);
        Assert.AreEqual(38100, response.Summaries.VatRateSums.Gross);
        Assert.AreEqual(30000, response.Summaries.TotalSums.Net);
        Assert.AreEqual(8100, response.Summaries.TotalSums.Vat);
        Assert.AreEqual(38100, response.Summaries.TotalSums.Gross);
    }
    
    [TestMethod]
    public async Task WebRequestTest()
    {
        var api = new SzamlazzHuApi();
        var request = CreateSampleRequest();
        string apiKey = Environment.GetEnvironmentVariable("SZAMLAZZ_HU_KEY");
        if (string.IsNullOrEmpty(apiKey))
            Assert.Inconclusive("Environment variable SZAMLAZZ_HU_KEY is not set.");
        request.AuthenticationData.ApiKey = apiKey;
        request.Header.InvoiceNumberPrefix = "NINCS";
        request.Header.IssueDate = DateTime.Now;
        request.Header.CompletionDate = DateTime.Now;
        request.Header.DueDate = DateTime.Now;
        request.Header.FeeCollection = true;
        request.Header.InvoiceTemplate = "Szla8cm";
        var response = await api.CreateInvoice(request);
        Assert.IsTrue(response.Success, response.ErrorMessage);
        var getInvoiceRequest = new GetInvoiceRequest();
        getInvoiceRequest.AuthenticationData.ApiKey = apiKey;
        getInvoiceRequest.InvoiceNumber = response.InvoiceNumber;
        var getInvoiceResponse = await api.GetInvoice(getInvoiceRequest);
        Assert.AreEqual(request.Seller.BankName, getInvoiceResponse.Seller.BankName);
        Assert.AreEqual(request.Seller.BankAccount, getInvoiceResponse.Seller.BankAccount);
        Assert.AreEqual(request.Header.Comment, getInvoiceResponse.InvoiceHeader.Comment);
        Assert.AreEqual(request.Header.CompletionDate.Date, getInvoiceResponse.InvoiceHeader.CompletionDate.Date);
        Assert.AreEqual(request.Header.Currency, getInvoiceResponse.InvoiceHeader.Currency);
        Assert.AreEqual(request.Header.DueDate.Date, getInvoiceResponse.InvoiceHeader.DueDate.Date);
        Assert.AreEqual(request.Header.ExchangeRate, getInvoiceResponse.InvoiceHeader.ExchangeRate);
        Assert.AreEqual(request.Header.FeeCollection, getInvoiceResponse.InvoiceHeader.FeeCollection);
        Assert.AreEqual(request.Header.InvoiceNumberPrefix, getInvoiceResponse.InvoiceHeader.InvoiceNumberPrefix);
        Assert.AreEqual(request.Header.IssueDate.Date, getInvoiceResponse.InvoiceHeader.IssueDate.Date);
        Assert.AreEqual(request.Header.Language, getInvoiceResponse.InvoiceHeader.Language);
        Assert.AreEqual(request.Header.PaymentType, getInvoiceResponse.InvoiceHeader.PaymentType);
        // TODO: InvoiceTemplate is not returned by the API yet
        //Assert.AreEqual(request.Header.InvoiceTemplate, getInvoiceResponse.InvoiceHeader.InvoiceTemplate);
        Assert.AreEqual(getInvoiceResponse.InvoiceHeader.InvoiceTemplate, null);
        Assert.AreEqual(request.Customer.Name, getInvoiceResponse.Customer.Name);
        Assert.AreEqual(request.Customer.CustomerAddress.Country, getInvoiceResponse.Customer.CustomerAddress.Country);
        Assert.AreEqual(request.Customer.CustomerAddress.PostalCode, getInvoiceResponse.Customer.CustomerAddress.PostalCode);
        Assert.AreEqual(request.Customer.CustomerAddress.City, getInvoiceResponse.Customer.CustomerAddress.City);
        Assert.AreEqual(request.Customer.CustomerAddress.StreetAddress, getInvoiceResponse.Customer.CustomerAddress.StreetAddress);
        Assert.AreEqual(request.Customer.EmailAddress, getInvoiceResponse.Customer.EmailAddress);
        Assert.AreEqual(request.Customer.Identification, getInvoiceResponse.Customer.Identification);
        Assert.AreEqual(request.Customer.TaxNumber, getInvoiceResponse.Customer.TaxNumber);
        Assert.AreEqual(request.Items.Count, getInvoiceResponse.InvoiceItems.Count);
        for (int i = 0; i < request.Items.Count; ++i)
        {
            Assert.AreEqual(request.Items[i].GrossAmount, getInvoiceResponse.InvoiceItems[i].GrossAmount);
            Assert.AreEqual(request.Items[i].Name, getInvoiceResponse.InvoiceItems[i].Name);
            Assert.AreEqual(request.Items[i].NetPrice, getInvoiceResponse.InvoiceItems[i].NetPrice);
            Assert.AreEqual(request.Items[i].Quantity, getInvoiceResponse.InvoiceItems[i].Quantity);
            Assert.AreEqual(request.Items[i].UnitOfQuantity, getInvoiceResponse.InvoiceItems[i].UnitOfQuantity);
            Assert.AreEqual(request.Items[i].UnitPrice, getInvoiceResponse.InvoiceItems[i].UnitPrice);
            Assert.AreEqual(request.Items[i].VatAmount, getInvoiceResponse.InvoiceItems[i].VatAmount);
            Assert.AreEqual(request.Items[i].VatRate, getInvoiceResponse.InvoiceItems[i].VatRate);
        }
        var deleteInvoiceRequest = new DeleteInvoiceRequest();
        deleteInvoiceRequest.AuthenticationData.ApiKey = apiKey;
        deleteInvoiceRequest.InvoiceNumber = response.InvoiceNumber;
        var deleteInvoiceResponse = await api.DeleteProFormaInvoice(deleteInvoiceRequest);
        Assert.IsTrue(deleteInvoiceResponse.Success, deleteInvoiceResponse.ErrorMessage);
    }

    private CreateInvoiceRequest CreateSampleRequest()
    {
        var request = new CreateInvoiceRequest();
        request.AuthenticationData.User = "teszt01";
        request.AuthenticationData.Password = "teszt01";
        request.AuthenticationData.ApiKey = "Please fill!";
        request.Header.IssueDate = new DateTime(2020, 1, 20);
        request.Header.CompletionDate = new DateTime(2020, 1, 20);
        request.Header.DueDate = new DateTime(2020, 1, 20);
        request.Header.Comment = "Invoce comment";
        request.Header.InvoiceTemplate = "Szla8cm";
        request.Header.PaymentType = "átutalás";
        request.Seller.BankName = "BB";
        request.Seller.BankAccount = "11111111-22222222-33333333";
        request.Seller.EmailSubject = "Invoice notification";
        request.Seller.EmailText = "mail text";
        request.Customer.Name = "Kovács & Társa Bt.";
        request.Customer.CustomerAddress.Country = "Magyarország";
        request.Customer.CustomerAddress.PostalCode = "2030";
        request.Customer.CustomerAddress.City = "Érd";
        request.Customer.CustomerAddress.StreetAddress = "Tárnoki út 23.";
        request.Customer.EmailAddress = "buyer@example.com";
        request.Customer.TaxNumber = "12345678-1-42";
        request.Customer.EuTaxNumber = "HU12345678";
        request.Customer.PostalName = "Kovács Bt. mailing name";
        request.Customer.PostalAddress.PostalCode = "2040";
        request.Customer.PostalAddress.City = "Budaörs";
        request.Customer.PostalAddress.StreetAddress = "Szivárvány utca 8.";
        request.Customer.Identification = "1234";
        request.Customer.PhoneNumber = "Tel:+3630-555-55-55, Fax:+3623-555-555";
        request.Customer.Comment = "Call extension 214 from the reception";
        request.Items = new List<InvoiceItem> {
            new InvoiceItem {
                Name = "Elado izé",
                Quantity = 1,
                UnitOfQuantity = "db",
                UnitPrice = 10000,
                VatRate = "27.0",
                NetPrice = 10000,
                VatAmount = 2700,
                GrossAmount = 12700,
                Comment = "lorem ipsum"
            },
            new InvoiceItem {
                Name = "Elado izé 2",
                Quantity = 2,
                UnitOfQuantity = "db",
                UnitPrice = 10000,
                VatRate = "27.0",
                NetPrice = 20000,
                VatAmount = 5400,
                GrossAmount = 25400,
                Comment = "lorem ipsum 2"
            }
        };
        return request;
    }
}
