using System.Web;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
	public class InvoiceItem : IHtmlEncodeable
	{
		public string Name { get; set; }
		public decimal Quantity { get; set; }
		public string UnitOfQuantity { get; set; }
		public decimal UnitPrice { get; set; }
		public string VatRate { get; set; }
		public decimal NetPrice { get; set; }
		public decimal VatAmount { get; set; }
		public decimal GrossAmount { get; set; }
		public string Comment { get; set; }

		public void HtmlEncode()
		{
			Name = HttpUtility.HtmlEncode(Name);
			UnitOfQuantity = HttpUtility.HtmlEncode(UnitOfQuantity);
			VatRate = HttpUtility.HtmlEncode(VatRate);
			Comment = HttpUtility.HtmlEncode(Comment);
		}
	}
}
