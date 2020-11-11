namespace SzamlazzHu
{
    public class DeleteInvoiceRequest
    {
        public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
        public string InvoiceNumber { get; set; }
    }
}