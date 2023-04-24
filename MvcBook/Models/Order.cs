using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Build.Framework;

namespace MvcBook.Models
{
    public class Order
    {
        [Key]
        [DisplayName("Transaction Id")]
        public string OrderId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string  Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Second Name is required")]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Post Code is required")]
        [DisplayName("Post Code")]
        public string PostCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
      ErrorMessage = "Email is is not valid.")]
        public string Email { get; set; }   
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }
        [ScaffoldColumn(false)]
        public string PaymentTransactionId { get; set; } = string.Empty;
        [ScaffoldColumn(false)]
        public bool HasBeenShipped { get; set; }
        public List<CartItem> ShoppingCartItems { get; set; } = new List<CartItem>();

    }
}
