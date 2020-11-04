using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Scriban;

namespace szamlazzhu
{
    public class XMLRenderer
    {
        public static string RenderRequest(CreateInvoiceRequest request)
        {
            const string path = "requestxml.sbn";
            var template = Template.Parse(File.ReadAllText(path), path);
            return template.Render(new { Request = request });
        }
    }
    public class CreateInvoiceRequest
    {
        public CreateInvoiceSettings Settings { get; set; } = new CreateInvoiceSettings();
        public InvoiceHeader Header {get; set; } = new InvoiceHeader();
        public Seller Seller { get; set; } = new Seller();
        public Customer Customer { get; set; } = new Customer();
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }

    public class CreateInvoiceSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public bool Electric { get; set; } = true;
        public bool DownloadInvoice { get; set; } = true;
    }
    public class InvoiceHeader
    {
        public DateTime CompletionDate { get; set; } = DateTime.Now;
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now;
        public PaymentType PaymentType { get; set; }
        public string PaymentTypeString => GetEnumDescription(PaymentType);
        public string Currency { get; set; } = "HUF";
        public InvoiceLanguage Language { get; set;} = InvoiceLanguage.Hu;
        public string Comment { get; set; }
        public string Bank { get; set; } = "MNB";
        public float ExchangeRate { get; set; }
        public string OrderNumber { get; set; }
        public bool DepositInvoice { get; set; }
        public bool FinalBill { get; set; }
        public bool CorrectionInvoice { get; set; }
        public string CorrectedInvoiceNumber { get; set; }
        public bool FeeCollection { get; set; }
        public string InvoiceNumberPrefix { get; set; }
        public Seller Seller { get; set; }
        public Customer Customer { get; set; }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
    public class Seller
    {
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string EmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailText { get; set; }
    }

    public class Customer
    {
        public Contact CustomerContact { get; set; } = new Contact();
        public Contact PostalContact { get; set; } = new Contact();
        public string EmailAddress { get; set; }
        public bool SendEmail { get; set; }
        public string TaxNumber { get; set; }
        public string Identification { get; set; }
        public string PhoneNumber { get; set; }
        public string Comment { get; set; }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }

    
    public class InvoiceItem
    {
        public string Name { get; set; }
        public float Quantity { get; set; }
        public string UnitOfQuantity { get; set; }
        public float UnitPrice { get; set; }
        public string VatRate { get; set; }
        public float NetPrice { get; set; }
        public float VatAmount { get; set; }
        public float GrossAmount { get; set; }
        public string Comment { get; set; }
    }

    public enum PaymentType
    {
        [Description("átutalás")]
        BankTransfer,
        [Description("készpénz")]
        Cash,
        [Description("bankkártya")]
        CreditCard,
        [Description("csekk")]
        Check,
        [Description("utánvét")]
        CashOnDelivery,
        [Description("ajándékutalvány")]
        GiftCard,
        [Description("barion")]
        Barion,
        [Description("barter")]
        Barter,
        [Description("csoportos beszedés")]
        DirectDebit,
        [Description("OTP Simple")]
        OTPSimple,
        [Description("kompenzáció")]
        Compensation,
        [Description("kupon")]
        Coupon,
        [Description("PayPal")]
        PayPal,
        [Description("PayU")]
        PayU,
        [Description("SZÉP kártya")]
        SZEPKartya,
        [Description("utalvány")]
        Voucher
    }

    public enum InvoiceLanguage
    {
        De, En, It, Hu, Fr, Ro, Sk, Hr
    }
}
