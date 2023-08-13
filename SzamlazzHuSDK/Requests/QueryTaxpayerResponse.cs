namespace SzamlazzHu
{
    public class QueryTaxpayerResponse
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool TaxpayerValid { get; set; }
        public Taxpayer Taxpayer { get; set; }
    }
}