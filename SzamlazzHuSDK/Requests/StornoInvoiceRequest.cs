using System.Collections.Generic;

namespace SzamlazzHu
{
    public class StornoInvoiceRequest
    {
        public string InvoiceNumber { get; set; }
        public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
        public CreateInvoiceSettings Settings { get; set; } = new CreateInvoiceSettings();
        public InvoiceHeader Header {get; set; } = new InvoiceHeader();
        public Seller Seller { get; set; } = new Seller();
        public Customer Customer { get; set; } = new Customer();
    }
}
