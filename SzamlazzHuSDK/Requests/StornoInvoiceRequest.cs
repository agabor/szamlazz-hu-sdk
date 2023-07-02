using System.Collections.Generic;
using System.Web;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
	public class StornoInvoiceRequest : IHtmlEncodeable
	{
		public string InvoiceNumber { get; set; }
		public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
		public CreateInvoiceSettings Settings { get; set; } = new CreateInvoiceSettings();
		public InvoiceHeader Header { get; set; } = new InvoiceHeader();
		public Seller Seller { get; set; } = new Seller();
		public Customer Customer { get; set; } = new Customer();

		public void HtmlEncode()
		{
			InvoiceNumber = HttpUtility.HtmlEncode(InvoiceNumber);
			Header.HtmlEncode();
			Seller.HtmlEncode();
			Customer.HtmlEncode();
		}
	}
}
