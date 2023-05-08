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
        public string CartId { get; set; } = string.Empty;
		[DataType(DataType.Date)]
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
        [RegularExpression(@"^[A-Z]{1,2}[0-9][0-9A-Z]? [0-9][A-Z]{2}$")]
        [DisplayName("Post Code")]
        public string PostCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        [RegularExpression(@"^\s*\(?(020[7,8]{1}\)?[ ]?[1-9]{1}[0-9{2}[ ]?[0-9]{4})|(0[1-8]{1}[0-9]{3}\)?[ ]?[1-9]{1}[0-9]{2}[ ]?[0-9]{3})\s*$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Not a valid email")]
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
