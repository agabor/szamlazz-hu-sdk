using System.IO;
using Scriban;

namespace SzamlazzHu
{
    public class XMLRenderer
    {
        public static MemoryStream RenderRequest(CreateInvoiceRequest request)
        {
            const string path = "createInvoiceRequest.sbn";
            var template = Template.Parse(File.ReadAllText(path), path);
            var xmlString = template.Render(new { Request = request });
            return CreateMemoryStream(xmlString);
        }

        internal static MemoryStream RenderRequest(GetInvoiceRequest request)
        {
            const string path = "getInvoiceRequest.sbn";
            var template = Template.Parse(File.ReadAllText(path), path);
            var xmlString = template.Render(new { Request = request });
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
    }
}
