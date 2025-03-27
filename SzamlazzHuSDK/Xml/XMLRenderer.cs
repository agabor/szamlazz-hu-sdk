using System;
using System.IO;
using System.Security;
using Newtonsoft.Json.Linq;
using Scriban;

namespace SzamlazzHu;

public class XMLRenderer
{
    public static MemoryStream RenderRequest(StornoInvoiceRequest request)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stornoInvoiceRequest.sbn");
        var template = Template.Parse(File.ReadAllText(path), path);
        var xmlString = template.Render(new { Request = EscapeRequest(request) });
        return CreateMemoryStream(xmlString);
    }

    public static MemoryStream RenderRequest(CreateInvoiceRequest request)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "createInvoiceRequest.sbn");
        var template = Template.Parse(File.ReadAllText(path), path);
        var xmlString = template.Render(new { Request = EscapeRequest(request) });
        return CreateMemoryStream(xmlString);
    }

    internal static MemoryStream RenderRequest(GetInvoiceRequest request)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"getInvoiceRequest.sbn");
        var template = Template.Parse(File.ReadAllText(path), path);
        var xmlString = template.Render(new { Request = EscapeRequest(request) });
        return CreateMemoryStream(xmlString);
    }
  
    private static MemoryStream CreateMemoryStream(string xmlString)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(xmlString);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    internal static MemoryStream RenderRequest(DeleteInvoiceRequest request)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deleteInvoiceRequest.sbn");
        var template = Template.Parse(File.ReadAllText(path), path);
        var xmlString = template.Render(new { Request = request });
        return CreateMemoryStream(xmlString);
    }

    private static T EscapeRequest<T>(T request)
    {
        var jObj = JObject.FromObject(request);
        EscapeJson(jObj);
        return jObj.ToObject<T>();
    }

    private static void EscapeJson(JToken token)
    {
        if (token.Type == JTokenType.Object)
        {
            foreach (var property in ((JObject)token).Properties())
            {
                EscapeJson(property.Value);
            }
        }
        else if (token.Type == JTokenType.Array)
        {
            foreach (var item in token.Children())
            {
                EscapeJson(item);
            }
        }
        else if (token.Type == JTokenType.String)
        {
            token.Replace(JValue.CreateString(SecurityElement.Escape(token.ToString())));
        }
    }

    internal static MemoryStream RenderRequest(QueryTaxpayerRequest request)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "queryTaxpayerRequest.sbn");
        var template = Template.Parse(File.ReadAllText(path), path);
        var xmlString = template.Render(new { Request = request });
        return CreateMemoryStream(xmlString);
    }

}
