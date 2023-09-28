using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace SzamlazzHu;

public class SzamlazzHuApi
{

    public async Task<CreateInvoiceResponse> CreateInvoice(CreateInvoiceRequest request)
    {
        using var xmlStream = XMLRenderer.RenderRequest(request);
        using var requestStream = CompressXmlStream(xmlStream);
        var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-xmlagentxmlfile");
        return XmlParser.ParseCreateInvoiceResponse(doc);
    }

    public async Task<StornoInvoiceResponse> StornoInvoice(StornoInvoiceRequest request)
    {
        using var xmlStream = XMLRenderer.RenderRequest(request);
        using var requestStream = CompressXmlStream(xmlStream);
        var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-szamla_agent_st");
        return XmlParser.ParseStornoInvoiceResponse(doc);
    }

    public async Task<GetInvoiceResponse> GetInvoice(GetInvoiceRequest request)
    {
        using var xmlStream = XMLRenderer.RenderRequest(request);
        using var requestStream = CompressXmlStream(xmlStream);
        var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-szamla_agent_xml");
        return XmlParser.ParseGetInvoiceResponse(doc);
    }
    public async Task<DeleteInvoiceResponse> DeleteProFormaInvoice(DeleteInvoiceRequest request)
    {
        using var xmlStream = XMLRenderer.RenderRequest(request);
        using var requestStream = CompressXmlStream(xmlStream);
        var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-szamla_agent_dijbekero_torlese");
        return XmlParser.ParseDeleteInvoiceResponse(doc);
    }

    private static MemoryStream CompressXmlStream(MemoryStream xmlStream)
    {
        var readerSettings = new XmlReaderSettings
        {
            IgnoreComments = true
        };
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

        using var response = await wr.GetResponseAsync();
        using var reader = new StreamReader(response.GetResponseStream());
        string xml = reader.ReadToEnd();
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
        catch
        {
            if (paramName != "action-szamla_agent_st")
                return null;
                
            var doc = new XmlDocument();
            var root = doc.CreateElement("root");
            var agentResponseElement = doc.CreateElement("AgentResponse");
            var invoiceNumberElement = doc.CreateElement("InvoiceNumber");
            var elements = System.Web.HttpUtility.HtmlDecode(xml.TrimEnd('\n')).Split(';');
            agentResponseElement.AppendChild(doc.CreateTextNode(elements[0]));
            invoiceNumberElement.AppendChild(doc.CreateTextNode(elements[1]));
            root.AppendChild(agentResponseElement);
            root.AppendChild(invoiceNumberElement);
            doc.AppendChild(root);
            return doc;
        }
    }
    public async Task<QueryTaxpayerResponse> QueryTaxpayer(QueryTaxpayerRequest request)
    {
        using (var xmlStream = XMLRenderer.RenderRequest(request))
        {
            using (var requestStream = CompressXmlStream(xmlStream))
            {
                var doc = await HttpUploadXmlFile("https://www.szamlazz.hu/szamla/", requestStream.ToArray(), "action-szamla_agent_taxpayer");
                return XmlParser.ParseQueryTaxpayerResponse(doc);
            }
        }

    }
    private static void WriteString(Stream rs, string text)
    {
        byte[] textbytes = Encoding.UTF8.GetBytes(text);
        rs.Write(textbytes, 0, textbytes.Length);
    }
}