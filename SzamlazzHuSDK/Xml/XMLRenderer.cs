using System;
using System.IO;
using Scriban;

namespace SzamlazzHu
{
    public class XMLRenderer
    {
        public static MemoryStream RenderRequest(StornoInvoiceRequest request)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stornoInvoiceRequest.sbn");
            var template = Template.Parse(File.ReadAllText(path), path);
            var xmlString = template.Render(new { Request = request });
            return CreateMemoryStream(xmlString);
        }

        public static MemoryStream RenderRequest(CreateInvoiceRequest request)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "createInvoiceRequest.sbn");
            var template = Template.Parse(File.ReadAllText(path), path);
            var xmlString = template.Render(new { Request = request });
            return CreateMemoryStream(xmlString);
        }

        internal static MemoryStream RenderRequest(GetInvoiceRequest request)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"getInvoiceRequest.sbn");
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

        internal static MemoryStream RenderRequest(DeleteInvoiceRequest request)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deleteInvoiceRequest.sbn");
            var template = Template.Parse(File.ReadAllText(path), path);
            var xmlString = template.Render(new { Request = request });
            return CreateMemoryStream(xmlString);
        }
    }
}
