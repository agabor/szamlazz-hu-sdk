using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace SzamlazzHu;

public static class XmlParser
{
    public static CreateInvoiceResponse ParseCreateInvoiceResponse(XmlDocument doc)
    {
        var root = doc.DocumentElement;
        string pdfString = GetString(root, "pdf");
        return new CreateInvoiceResponse
        {
            Success = GetBool(root, "sikeres"),
            ErrorCode = GetInt(root, "hibakod"),
            ErrorMessage = GetString(root, "hibauzenet"),
            InvoiceNumber = GetString(root, "szamlaszam"),
            NetPrice = GetDecimal(root, "szamlanetto"),
            GrossPrice = GetDecimal(root, "szamlabrutto"),
            Receivable = GetDecimal(root, "kintlevoseg"),
            CustomerAccountUrl = GetString(root, "vevoifiokurl"),
            InvoicePdf = pdfString != null ? Convert.FromBase64String(pdfString) : null
        };
    }

    public static StornoInvoiceResponse ParseStornoInvoiceResponse(XmlDocument doc)
    {
        var root = doc.DocumentElement;
        return new StornoInvoiceResponse
        {
            AgentResponse = GetString(root, "AgentResponse"),
            InvoiceNumber = GetString(root, "InvoiceNumber")
        };
    }

    public static GetInvoiceResponse ParseGetInvoiceResponse(XmlDocument doc)
    {
        var root = doc.DocumentElement;
        string pdfString = GetString(root, "pdf");
        var response = new GetInvoiceResponse
        {
            InvoicePdf = pdfString != null ? Convert.FromBase64String(pdfString) : null
        };            

        var tetelek = root["tetelek"];
        if (tetelek != null)
            foreach (var item in tetelek.ChildNodes)
            {
                response.InvoiceItems.Add(ParseInvoiceItem((XmlNode)item));
            }

        var kifizetesek = root["kifizetesek"];
        if (kifizetesek != null)
            foreach (var payment in kifizetesek.ChildNodes)
            {
                response.PaymentItems.Add(ParsePaymentItem((XmlNode)payment));
            }

        response.InvoiceHeader = ParseInvoiceHeader(root["alap"]);
        response.Customer = ParseCustomer(root["vevo"]);
        response.Seller = ParseSeller(root["szallito"]);
        response.Summaries = ParseSummaries(root["osszegek"]);
        return response;
    }

    public static QueryTaxpayerResponse ParseQueryTaxpayerResponse(XmlDocument doc)
    {
        var xml = doc.OuterXml.Replace("ns2:", "").Replace("ns3:", "");
        doc = new XmlDocument();
        doc.LoadXml(xml);
        var root = doc.DocumentElement;
        var result = root["result"];
        if (result == null)
            Console.WriteLine(doc.OuterXml);
        var response = new QueryTaxpayerResponse
        {
            Success = GetString(result, "funcCode") == "OK",
            ErrorCode = GetInt(result, "errorCode"),
            ErrorMessage = GetString(result, "message")
        };

        if (!response.Success)
        {
            return response;
        }
        response.TaxpayerValid = GetBool(root, "taxpayerValidity");

        if (!response.TaxpayerValid)
        {
            return response;
        }

        response.Taxpayer = ParseTaxpayer(root["taxpayerData"]);
        return response;
    }

    internal static DeleteInvoiceResponse ParseDeleteInvoiceResponse(XmlDocument doc)
    {
        var root = doc.DocumentElement;
        return new DeleteInvoiceResponse
        {
            Success = GetBool(root, "sikeres"),
            ErrorCode = GetInt(root, "hibakod"),
            ErrorMessage = GetString(root, "hibauzenet")
        };
    }

    private static Seller ParseSeller(XmlNode node)
    {
        return new Seller
        {
            Name = GetString(node, "nev"),
            SellerAddress = ParseAddress(node["cim"]),
            BankName = GetString(node["bank"], "nev"),
            BankAccount = GetString(node["bank"], "bankszamla"),
            TaxNumber = GetString(node, "adoszam"),
            EuTaxNumber = GetString(node, "adoszameu"),
        };
    }

    private static Customer ParseCustomer(XmlNode node)
    {
        return new Customer
        {
            Name = GetString(node, "nev"),
            Identification = GetString(node, "azonosito"),
            CustomerAddress = ParseAddress(node["cim"]),
            EmailAddress = GetString(node, "email"),
            TaxNumber = GetString(node, "adoszam"),
            EuTaxNumber = GetString(node, "adoszamEU")
        };
    }

    private static Taxpayer ParseTaxpayer(XmlNode node)
    {
        var taxpayer = new Taxpayer
        {
            TaxpayerName = GetString(node, "taxpayerName"),
            TaxNumberDetail = ParseTaxNumber(node["taxNumberDetail"]),
            Incorporation = GetString(node, "incorporation")
        };

        if (node["taxpayerShortName"] != null)
            taxpayer.TaxpayerShortName = GetString(node, "taxpayerShortName");

        if (node["vatGroupMembership"] != null)
            taxpayer.VatGroupMembership = ParseTaxNumber(node["vatGroupMembership"]);

        if (node["taxpayerAddressList"] != null)
            taxpayer.AddressList = ParseTaxpayerAddressList(node["taxpayerAddressList"]);

        return taxpayer;
    }

    private static Address ParseAddress(XmlNode node)
    {
        return new Address
        {
            Country = GetString(node, "orszag"),
            PostalCode = GetString(node, "irsz"),
            City = GetString(node, "telepules"),
            StreetAddress = GetString(node, "cim")
        };
    }

    private static List<TaxpayerAddressItem> ParseTaxpayerAddressList(XmlNode node)
    {
        var taxpayerList = new List<TaxpayerAddressItem>();

        if (node == null) return taxpayerList;

        foreach (XmlNode item in node.ChildNodes)
        {
            taxpayerList.Add(ParseTaxpayerAddressItem(item));
        }

        return taxpayerList;
    }

    private static TaxpayerAddressItem ParseTaxpayerAddressItem(XmlNode node)
    {
        var address = node["taxpayerAddress"];

        string streetAddress = GetString(address, "streetName") + " "
                                + GetString(address, "publicPlaceCategory");

        if (address["number"] != null)
            streetAddress += " " + GetString(address, "number");
        if (address["building"] != null)
            streetAddress += "/" + GetString(address, "building");
        if (address["staircase"] != null)
            streetAddress += " " + GetString(address, "staircase") + " lh.";
        if (address["floor"] != null)
            streetAddress += " " + GetString(address, "floor") + " em.";
        if (address["door"] != null)
            streetAddress += " " + GetString(address, "door");
        if (address["lotNumber"] != null)
            streetAddress += "HRSZ: " + GetString(address, "lotNumber");

        return new TaxpayerAddressItem
        {
            AddressType = GetString(node, "taxpayerAddressType"),
            Address = new Address
            {
                Country = GetString(address, "countryCode"),
                PostalCode = GetString(address, "postalCode"),
                City = GetString(address, "city"),
                StreetAddress = streetAddress
            }
        };
    }

    private static TaxNumber ParseTaxNumber(XmlNode node)
    {
        return new TaxNumber
        {
            TaxpayerId = GetString(node, "taxpayerId"),
            VatCode = GetString(node, "vatCode"),
            CountryCode = GetString(node, "countyCode")
        };
    }

    private static InvoiceHeader ParseInvoiceHeader(XmlNode node)
    {
        return new InvoiceHeader
        {
            InvoiceNumber = GetString(node, "szamlaszam"),
            CompletionDate = GetDate(node, "telj"),
            IssueDate = GetDate(node, "kelt"),
            DueDate = GetDate(node, "fizh"),
            PaymentType = GetString(node, "fizmod"),
            Language = GetEnum<InvoiceLanguage>(node, "nyelv"),
            Comment = GetString(node, "megjegyzes"),
            FeeCollection = GetString(node, "tipus").ToLower() == "d",
            InvoiceNumberPrefix = GetPrefix(GetString(node, "szamlaszam")),
            OrderNumber = GetString(node, "rendelesszam"),
            InvoiceTemplate = GetString(node, "szamlaSablon"),
            Currency = GetString(node, "devizanem"),
            InvoiceType = GetString(node, "tipus"),
            ReferenceInvoiceNumber = GetString(node, "hivszamlaszam"),
        };
    }

    private static Summaries ParseSummaries(XmlElement element)
    {
        XmlNode vatRateSums = element["afakulcsossz"];
        XmlNode totalSums = element["totalossz"];
        return new Summaries
        {
            VatRateSums = new VatRateSums
            {
                VatRate = GetDecimal(vatRateSums, "afakulcs"),
                Net = GetDecimal(vatRateSums, "netto"),
                Vat = GetDecimal(vatRateSums, "afa"),
                Gross = GetDecimal(vatRateSums, "brutto")
            },
            TotalSums = new TotalSums
            {
                Net = GetDecimal(totalSums, "netto"),
                Vat = GetDecimal(totalSums, "afa"),
                Gross = GetDecimal(totalSums, "brutto")
            }
        };
    }

    private static string GetPrefix(string invoiceNumber)
    {
        var parts = invoiceNumber.Split('-');
        if (parts.Length == 4)
            return parts[1];
        if (parts[0] == "D")
            return parts[1];
        return parts[0];
    }

    private static DateTime GetDate(XmlNode node, string tagName)
    {
        return DateTime.ParseExact(GetString(node, tagName), "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static T GetEnum<T>(XmlNode node, string tagName)
    {
        var stringVal = GetString(node, tagName);
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            FieldInfo fi = typeof(T).GetField(value.ToString());

            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes.FirstOrDefault().Description == stringVal)
            {
                return value;
            }
        }
        return (T)Enum.GetValues(typeof(T)).GetValue(0);
    }

    private static InvoiceItem ParseInvoiceItem(XmlNode node)
    {
        return new InvoiceItem
        {
            Name = GetString(node, "nev"),
            Quantity = GetDecimal(node, "mennyiseg"),
            UnitOfQuantity = GetString(node, "mennyisegiegyseg"),
            UnitPrice = GetDecimal(node, "nettoegysegar"),
            VatRate = GetString(node, "afakulcs"),
            NetPrice = GetDecimal(node, "netto"),
            VatAmount = GetDecimal(node, "afa"),
            GrossAmount = GetDecimal(node, "brutto"),
            Comment = GetString(node, "megjegyzes")
        };
    }

    private static PaymentItem ParsePaymentItem(XmlNode node)
    {
        return new PaymentItem
        {
            Date = GetDate(node, "datum"),
            Title = GetString(node, "jogcim"),
            Amount = GetDecimal(node, "osszeg"),
            Comment = GetString(node, "megjegyzes"),
            BankAccountNumber = GetString(node, "bankszamlaszam")
        };
    }

    private static float GetFloat(XmlNode doc, string tagName)
    {
        float.TryParse(GetString(doc, tagName), out float value);
        return value;
    }
    private static int GetInt(XmlNode doc, string tagName)
    {
        int.TryParse(GetString(doc, tagName), out int value);
        return value;
    }

    private static bool GetBool(XmlNode doc, string tagName)
    {
        bool.TryParse(GetString(doc, tagName), out bool value);
        return value;
    }
    private static string GetString(XmlNode doc, string tagName)
    {
        return doc[tagName]?.FirstChild?.Value;
    }
    private static decimal GetDecimal(XmlNode doc, string tagName)
    {
        decimal.TryParse(GetString(doc, tagName), out decimal value);
        return value;
    }

}