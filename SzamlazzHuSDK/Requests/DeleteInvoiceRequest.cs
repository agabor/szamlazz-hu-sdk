using System.Web;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
    public class DeleteInvoiceRequest:IHtmlEncodeable
    {
        public AuthenticationData AuthenticationData { get; set; } = new AuthenticationData();
        public string InvoiceNumber { get; set; }

		public void HtmlEncode()
		{
			InvoiceNumber= HttpUtility.HtmlEncode(InvoiceNumber);	
		}
	}
}