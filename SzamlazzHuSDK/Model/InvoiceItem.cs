namespace SzamlazzHu
{
    public class InvoiceItem
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfQuantity { get; set; }
        public float UnitPrice { get; set; }
        public string VatRate { get; set; }
        public decimal NetPrice { get; set; }
        public decimal VatAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public string Comment { get; set; }
    }
}
