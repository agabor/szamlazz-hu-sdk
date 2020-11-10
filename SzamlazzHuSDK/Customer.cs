namespace SzamlazzHu
{
    public class Customer
    {
        public Address CustomerAddress { get; set; } = new Address();
        public Address PostalAddress { get; set; } = new Address();
        public string EmailAddress { get; set; }
        public bool SendEmail { get; set; }
        public string TaxNumber { get; set; }
        public string Identification { get; set; }
        public string PhoneNumber { get; set; }
        public string Comment { get; set; }
    }
}
