using System;
using System.Collections.Generic;
using System.Text;

namespace SzamlazzHu
{
    public class PaymentItem
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public float Amount { get; set; }
        public string Comment { get; set; }
        public string BankAccountNumber { get; set; }
    }
}
