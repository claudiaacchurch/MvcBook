using System.ComponentModel.DataAnnotations;

namespace MvcBook.Models
{
    public class CartItem
    {
        [Key]
        //Id of cart item (which book added)
        public string ItemId { get; set; } = string.Empty;
        //Id for user associated with ItemId
        public string CartId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
    }
}
