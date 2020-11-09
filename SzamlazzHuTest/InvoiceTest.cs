using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.XmlDiffPatch;
using SzamlazzHu;

namespace SzamlazzHuTest
{
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
                originalDoc.Load("testInvoice.xml");
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
        public async Task WebRequestTest()
        {
            var api = new SzamlazzHuApi();
            var request = CreateSampleRequest();
            request.AuthenticationData.User = "";
            request.AuthenticationData.Password = "";
            request.AuthenticationData.ApiKey = Environment.GetEnvironmentVariable("SZAMLAZZ_HU_KEY");
            request.Header.InvoiceNumberPrefix = "NINCS";
            request.Header.IssueDate = DateTime.Now;
            request.Header.CompletionDate = DateTime.Now;
            request.Header.DueDate = DateTime.Now;
            var response = await api.CreateInvoice(request);
            Assert.IsTrue(response.Success);
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
            request.Seller.BankName = "BB";
            request.Seller.BankAccount = "11111111-22222222-33333333";
            request.Seller.EmailSubject = "Invoice notification";
            request.Seller.EmailText = "mail text";
            request.Customer.CustomerContact.Name = "Kovacs Bt.";
            request.Customer.CustomerContact.PostalCode = "2030";
            request.Customer.CustomerContact.City = "Érd";
            request.Customer.CustomerContact.Address = "Tárnoki út 23.";
            request.Customer.EmailAddress = "buyer@example.com";
            request.Customer.TaxNumber = "12345678-1-42";
            request.Customer.PostalContact.Name = "Kovács Bt. mailing name";
            request.Customer.PostalContact.PostalCode = "2040";
            request.Customer.PostalContact.City = "Budaörs";
            request.Customer.PostalContact.Address = "Szivárvány utca 8.";
            request.Customer.Identification = "1234";
            request.Customer.PhoneNumber = "Tel:+3630-555-55-55, Fax:+3623-555-555";
            request.Customer.Comment = "Call extension 214 from the reception";
            request.Items = new List<InvoiceItem> {
                new InvoiceItem {
                    Name = "Elado izé",
                    Quantity = 1,
                    UnitOfQuantity = "db",
                    UnitPrice = 10000,
                    VatRate = "27",
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
                    VatRate = "27",
                    NetPrice = 20000,
                    VatAmount = 5400,
                    GrossAmount = 25400,
                    Comment = "lorem ipsum 2"
                }
            };
            return request;
        }
    }
}
