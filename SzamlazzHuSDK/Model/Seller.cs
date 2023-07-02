using System.Web;
using SzamlazzHuSDK.Model;

namespace SzamlazzHu
{
	public class Seller : IHtmlEncodeable
	{
		public string BankName { get; set; }
		public string BankAccount { get; set; }
		public string EmailAddress { get; set; }
		public string EmailSubject { get; set; }
		public string EmailText { get; set; }

		public void HtmlEncode()
		{
			BankName = HttpUtility.HtmlEncode(BankName);
			BankAccount = HttpUtility.HtmlEncode(BankAccount);
			EmailAddress = HttpUtility.HtmlEncode(EmailAddress);
			EmailSubject = HttpUtility.HtmlEncode(EmailSubject);
			EmailText = HttpUtility.HtmlEncode(EmailText);
		}
	}
}
