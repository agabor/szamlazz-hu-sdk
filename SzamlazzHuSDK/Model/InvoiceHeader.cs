using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SzamlazzHu
{
    public class InvoiceHeader
    {
        public DateTime CompletionDate { get; set; } = DateTime.Now;
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now;
        public PaymentType PaymentType { get; set; }
        public string PaymentTypeString => GetEnumDescription(PaymentType);
        public string Currency { get; set; } = "HUF";
        public InvoiceLanguage Language { get; set;} = InvoiceLanguage.Hungarian;
        public string LanguageString => GetEnumDescription(Language);
        public string Comment { get; set; }
        public string Bank { get; set; } = "MNB";
        public decimal ExchangeRate { get; set; }
        public string OrderNumber { get; set; }
        public bool DepositInvoice { get; set; }
        public bool FinalBill { get; set; }
        public bool CorrectionInvoice { get; set; }
        public string CorrectedInvoiceNumber { get; set; }
        public bool FeeCollection { get; set; }
        public string InvoiceNumberPrefix { get; set; }
        public string InvoiceType { get; set; }
        public bool Paid { get; set; } = false;

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
}
