namespace SzamlazzHu
{
    public class Seller
    {
        public string Name { get; set; }
        public Address SellerAddress { get; set; } = new Address();
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string EmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailText { get; set; }
        public string TaxNumber { get; set; }
        public string EuTaxNumber { get; set; }
    }
}
