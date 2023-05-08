namespace MvcBook.Models
{
    public class PaymentModel
    {
        public string ItemName { get; set; } = string.Empty;
        public decimal ItemPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
