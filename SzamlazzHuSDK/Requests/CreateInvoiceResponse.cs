namespace SzamlazzHu
{
    public class CreateInvoiceResponse
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal NetPrice { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal Receivable { get; set; }
        public string CustomerAccountUrl { get; set; }
        public byte[] InvoicePdf { get; set; }
    }
}
