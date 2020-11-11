namespace SzamlazzHu
{
    public class GetInvoiceRequest
    {
        public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public bool Pdf { get; set; }
    }
}