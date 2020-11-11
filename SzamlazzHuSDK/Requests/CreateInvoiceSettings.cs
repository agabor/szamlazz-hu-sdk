namespace SzamlazzHu
{
    public class AuthenticationData
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
    }
    
    public class CreateInvoiceSettings
    {
        public bool Electric { get; set; } = true;
        public bool DownloadInvoice { get; set; } = true;
    }
}
