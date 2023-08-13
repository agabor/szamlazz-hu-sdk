using System.Collections.Generic;

namespace SzamlazzHu
{
    public class QueryTaxpayerRequest
    {
        public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
        public string TaxpayerId { get; set; }
    }
}
