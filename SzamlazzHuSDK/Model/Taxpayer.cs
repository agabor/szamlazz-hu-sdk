using System.Collections.Generic;

namespace SzamlazzHu
{
    public class Taxpayer
    {
        public string TaxpayerName { get; set; }
        public string TaxpayerShortName { get; set; }
        public TaxNumber TaxNumberDetail { get; set; }
        public TaxNumber VatGroupMembership { get; set; }
        public string Incorporation { get; set; }
        public List<TaxpayerAddressItem> AddressList { get; set; }
    }
}
