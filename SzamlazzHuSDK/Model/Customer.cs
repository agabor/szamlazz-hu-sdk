using System.Web;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
	public class Customer : IHtmlEncodeable
	{
		public string Name { get; set; }
		public Address CustomerAddress { get; set; } = new Address();
		public string PostalName { get; set; }
		public Address PostalAddress { get; set; } = new Address();
		public string EmailAddress { get; set; }
		public bool SendEmail { get; set; }
		public string TaxNumber { get; set; }
		public string EuTaxNumber { get; set; }
		public string Identification { get; set; }
		public string PhoneNumber { get; set; }
		public string Comment { get; set; }

		public void HtmlEncode()
		{
			Name = HttpUtility.HtmlEncode(Name);
			PostalName = HttpUtility.HtmlEncode(PostalName);
			EmailAddress = HttpUtility.HtmlEncode(EmailAddress);
			TaxNumber = HttpUtility.HtmlEncode(TaxNumber);
			EuTaxNumber = HttpUtility.HtmlEncode(EuTaxNumber);
			Identification = HttpUtility.HtmlEncode(Identification);
			PhoneNumber = HttpUtility.HtmlEncode(PhoneNumber);
			Comment = HttpUtility.HtmlEncode(Comment);
			CustomerAddress.HtmlEncode();
			PostalAddress.HtmlEncode();
		}
	}
}
