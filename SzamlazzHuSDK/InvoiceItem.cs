namespace SzamlazzHu
{
    public class InvoiceItem
    {
        public string Name { get; set; }
        public float Quantity { get; set; }
        public string UnitOfQuantity { get; set; }
        public float UnitPrice { get; set; }
        public string VatRate { get; set; }
        public float NetPrice { get; set; }
        public float VatAmount { get; set; }
        public float GrossAmount { get; set; }
        public string Comment { get; set; }
    }
}
