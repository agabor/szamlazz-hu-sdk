using System.ComponentModel;

namespace SzamlazzHu
{
    public enum PaymentType
    {
        [Description("átutalás")]
        BankTransfer,
        [Description("készpénz")]
        Cash,
        [Description("Bankkártyás fizetés")]
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
}
