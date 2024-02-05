using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

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
        var content = new MultipartFormDataContent(boundary);

        var fileContent = new ByteArrayContent(file);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml");
        content.Add(fileContent, paramName, "data.xml");

        using var client = new HttpClient();
        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var xml = await response.Content.ReadAsStringAsync();
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
        else
        {
            throw new Exception($"Error: {response.StatusCode}");
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