using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;

namespace SzamlazzHu
{
    public class CreateInvoiceResponse
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string InvoiceNumber { get; set; }
        public float NetPrice { get; set; }
        public float GrossPrice { get; set; }
        public float Receivable { get; set; }
        public string CustomerAccountUrl { get; set; }
        public byte[] InvoicePdf { get; set; }
    }

    public class SzamlazzHuApi
    {

        public async Task<CreateInvoiceResponse> CreateInvoice(CreateInvoiceRequest request)
        {
            using (var xmlStream = XMLRenderer.RenderRequest(request))
            {
                using (var requestStream = CompressXmlStream(xmlStream))
                {
                    var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-xmlagentxmlfile");

                    return new CreateInvoiceResponse
                    {
                        Success = GetBool(doc, "sikeres"),
                        ErrorCode = GetInt(doc, "hibakod"),
                        ErrorMessage = GetString(doc, "hibauzenet"),
                        InvoiceNumber = GetString(doc, "szamlaszam"),
                        NetPrice = GetFloat(doc, "szamlanetto"),
                        GrossPrice = GetFloat(doc, "szamlabrutto"),
                        Receivable = GetFloat(doc, "kintlevoseg"),
                        CustomerAccountUrl = GetString(doc, "vevoifiokurl"),
                        InvoicePdf = Convert.FromBase64String(GetString(doc, "pdf"))
                    };
                }
            }
        }

        private static float GetFloat(XmlDocument doc, string tagName)
        {
            float.TryParse(GetString(doc, tagName), out float value);
            return value;
        }
        private static int GetInt(XmlDocument doc, string tagName)
        {
            int.TryParse(GetString(doc, tagName), out int value);
            return value;
        }
        private static bool GetBool(XmlDocument doc, string tagName)
        {
            bool.TryParse(GetString(doc, tagName), out bool value);
            return value;
        }
        private static string GetString(XmlDocument doc, string tagName)
        {
            return doc.GetElementsByTagName(tagName).Item(0)?.FirstChild.Value;
        }

        private static MemoryStream CompressXmlStream(MemoryStream xmlStream)
        {
            var readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            var reader = XmlReader.Create(xmlStream, readerSettings);
            var doc = new XmlDocument();
            doc.Load(reader);
            var requestStream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(requestStream);
            doc.WriteTo(xmlWriter);
            xmlWriter.Close();
            requestStream.Position = 0;
            return requestStream;
        }

        private async Task<XmlDocument> HttpUploadXmlFile(string url, byte[] file, string paramName)
        {
            string boundary = $"---------------------------{DateTime.Now.Ticks.ToString("x")}";

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = $"multipart/form-data; boundary={boundary}";
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            Stream requstStream = wr.GetRequestStream();

            WriteString(requstStream, $"\r\n--{boundary}\r\n");

            string header = $"Content-Disposition: form-data; name=\"{paramName}\"; filename=\"data.xml\"\nContent-Type: text/xml\r\n\r\n";
            WriteString(requstStream, header);

            requstStream.Write(file, 0, file.Length);

            WriteString(requstStream, $"\r\n--{boundary}--\r\n");
            requstStream.Close();

            using (var response = await wr.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var doc = new XmlDocument();
                    string xml = reader.ReadToEnd();
                    doc.LoadXml(xml);
                    return doc;
                }
            }
        }

        private static void WriteString(Stream rs, string text)
        {
            byte[] textbytes = Encoding.UTF8.GetBytes(text);
            rs.Write(textbytes, 0, textbytes.Length);
        }
    }
}