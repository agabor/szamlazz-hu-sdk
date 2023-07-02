using System.Collections.Generic;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{

	public class CreateInvoiceRequest : IHtmlEncodeable
	{
		public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
		public CreateInvoiceSettings Settings { get; set; } = new CreateInvoiceSettings();
		public InvoiceHeader Header { get; set; } = new InvoiceHeader();
		public Seller Seller { get; set; } = new Seller();
		public Customer Customer { get; set; } = new Customer();
		public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

		public void HtmlEncode()
		{
			Customer.HtmlEncode();
			Seller.HtmlEncode();
			Items.ForEach(x => x.HtmlEncode());
			Header.HtmlEncode();
		}
	}
}
