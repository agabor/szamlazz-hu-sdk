using System.Web;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
	public class Address : IHtmlEncodeable
	{
		public string Country { get; set; }
		public string PostalCode { get; set; }
		public string City { get; set; }
		public string StreetAddress { get; set; }

		public void HtmlEncode()
		{
			Country = HttpUtility.HtmlEncode(Country);
			City = HttpUtility.HtmlEncode(City);
			StreetAddress = HttpUtility.HtmlEncode(StreetAddress);
			PostalCode = HttpUtility.HtmlEncode(PostalCode);
		}
	}
}
