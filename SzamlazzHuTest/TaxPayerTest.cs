using System;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SzamlazzHu;

namespace SzamlazzHuTest;

[TestClass]
public class TaxPayerTest
{
    [TestMethod]
    public void ParseQueryTaxpayerResponse()
    {
        var taxPayerDoc = new XmlDocument();
        taxPayerDoc.Load("testQueryTaxpayerResponse.xml");
        var taxPayerResponse = XmlParser.ParseQueryTaxpayerResponse(taxPayerDoc);
        Assert.AreEqual("Code Sharp Kft.", taxPayerResponse.Taxpayer.TaxpayerShortName);
    }

    [TestMethod]
    public async Task QueryTaxpayer()
    {
        var api = new SzamlazzHuApi();
        string apiKey = Environment.GetEnvironmentVariable("SZAMLAZZ_HU_KEY");
        if (string.IsNullOrEmpty(apiKey))
            Assert.Inconclusive("Environment variable SZAMLAZZ_HU_KEY is not set.");
        var taxPayerResponse = await api.QueryTaxpayer(new QueryTaxpayerRequest { AuthenticationData = new AuthenticationData { ApiKey = apiKey }, TaxpayerId = "27273209" });
        Assert.AreEqual("Code Sharp Kft.", taxPayerResponse.Taxpayer.TaxpayerShortName);
    }
}