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
}