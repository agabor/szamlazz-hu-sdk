using System.Collections.Generic;

namespace SzamlazzHu
{
    public class GetInvoiceResponse
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public byte[] InvoicePdf { get; internal set; }
        public InvoiceHeader InvoiceHeader { get; set; }
        public Seller Seller { get; set; }
        public Customer Customer { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public List<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();
        public Summaries Summaries { get; set; }
    }
}