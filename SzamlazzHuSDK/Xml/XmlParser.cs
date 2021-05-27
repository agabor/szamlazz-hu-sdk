using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
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
                NetPrice = GetFloat(root, "szamlanetto"),
                GrossPrice = GetFloat(root, "szamlabrutto"),
                Receivable = GetFloat(root, "kintlevoseg"),
                CustomerAccountUrl = GetString(root, "vevoifiokurl"),
                InvoicePdf = pdfString != null ? Convert.FromBase64String(pdfString) : null
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
                BankName = GetString(node["bank"], "nev"),
                BankAccount = GetString(node["bank"], "bankszamla")
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
                TaxNumber = GetString(node, "adoszam")
            };
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

        private static InvoiceHeader ParseInvoiceHeader(XmlNode node)
        {
            return new InvoiceHeader
            {
                CompletionDate = GetDate(node, "telj"),
                IssueDate = GetDate(node, "kelt"),
                DueDate = GetDate(node, "fizh"),
                PaymentType = GetEnum<PaymentType>(node, "fizmodunified"),
                Language = GetEnum<InvoiceLanguage>(node, "nyelv"),
                Comment = GetString(node, "megjegyzes"),
                FeeCollection = GetString(node, "tipus").ToLower() == "d",
                InvoiceNumberPrefix = GetPrefix(GetString(node, "szamlaszam"))
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
                    VatRate = GetFloat(vatRateSums, "afakulcs"),
                    Net = GetFloat(vatRateSums, "netto"),
                    Vat = GetFloat(vatRateSums, "afa"),
                    Gross = GetFloat(vatRateSums, "brutto")
                },
                TotalSums = new TotalSums
                {
                    Net = GetFloat(totalSums, "netto"),
                    Vat = GetFloat(totalSums, "afa"),
                    Gross = GetFloat(totalSums, "brutto")
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
                Quantity = GetFloat(node, "mennyiseg"),
                UnitOfQuantity = GetString(node, "mennyisegiegyseg"),
                UnitPrice = GetFloat(node, "nettoegysegar"),
                VatRate = GetString(node, "afakulcs"),
                NetPrice = GetFloat(node, "netto"),
                VatAmount = GetFloat(node, "afa"),
                GrossAmount = GetFloat(node, "brutto"),
                Comment = GetString(node, "megjegyzes")
            };
        }

        private static PaymentItem ParsePaymentItem(XmlNode node)
        {
            return new PaymentItem
            {
                Date = GetDate(node, "datum"),
                Title = GetString(node, "jogcim"),
                Amount = GetFloat(node, "osszeg"),
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
            return doc[tagName]?.FirstChild.Value;
        }

    }
}