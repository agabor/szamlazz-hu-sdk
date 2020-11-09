namespace SzamlazzHu
{
    public class Customer
    {
        public Contact CustomerContact { get; set; } = new Contact();
        public Contact PostalContact { get; set; } = new Contact();
        public string EmailAddress { get; set; }
        public bool SendEmail { get; set; }
        public string TaxNumber { get; set; }
        public string Identification { get; set; }
        public string PhoneNumber { get; set; }
        public string Comment { get; set; }
    }
}
