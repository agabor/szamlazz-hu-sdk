using System.Collections.Generic;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
    public class GetInvoiceResponse
    {
        public byte[] InvoicePdf { get; internal set; }
        public InvoiceHeader InvoiceHeader { get; set; }
        public Seller Seller { get; set; }
        public Customer Customer { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public List<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();
    }
}