using System.Web;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
	public class GetInvoiceRequest : IHtmlEncodeable
	{
		public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
		public string InvoiceNumber { get; set; }
		public string OrderNumber { get; set; }
		public bool Pdf { get; set; }

		public void HtmlEncode()
		{
			InvoiceNumber = HttpUtility.HtmlEncode(InvoiceNumber);
			OrderNumber = HttpUtility.HtmlEncode(OrderNumber);
		}
	}
}