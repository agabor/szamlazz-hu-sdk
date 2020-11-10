using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;

namespace SzamlazzHu
{

    public class SzamlazzHuApi
    {

        public async Task<CreateInvoiceResponse> CreateInvoice(CreateInvoiceRequest request)
        {
            using (var xmlStream = XMLRenderer.RenderRequest(request))
            {
                using (var requestStream = CompressXmlStream(xmlStream))
                {
                    var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-xmlagentxmlfile");
                    var root = doc.DocumentElement;
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
                        InvoicePdf = request.Settings.DownloadInvoice ? Convert.FromBase64String(GetString(root, "pdf")) : null
                    };
                }
            }
        }

        public async Task<GetInvoiceResponse> GetInvoice(GetInvoiceRequest request)
        {
            using (var xmlStream = XMLRenderer.RenderRequest(request))
            {
                using (var requestStream = CompressXmlStream(xmlStream))
                {
                    var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-szamla_agent_xml");
                    return new GetInvoiceResponse
                    {
                        InvoicePdf = request.Pdf ? Convert.FromBase64String(GetString(doc, "pdf")) : null
                    };
                }
            }
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
                    string xml = reader.ReadToEnd();
                    try
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(xml);
                        return doc;
                    } catch {
                        return null;
                    }
                }
            }
        }

        private static void WriteString(Stream rs, string text)
        {
            byte[] textbytes = Encoding.UTF8.GetBytes(text);
            rs.Write(textbytes, 0, textbytes.Length);
        }
    }

    public class GetInvoiceRequest
    {
        public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public bool Pdf { get; set; }
    }

    public class GetInvoiceResponse
    {
        public byte[] InvoicePdf { get; internal set; }
    }
}