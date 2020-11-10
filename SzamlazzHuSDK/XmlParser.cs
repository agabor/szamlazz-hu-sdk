using System;
using System.Xml;

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
            return new GetInvoiceResponse
            {
                InvoicePdf = pdfString != null ? Convert.FromBase64String(pdfString) : null
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