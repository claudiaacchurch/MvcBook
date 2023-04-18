using Microsoft.AspNetCore.Mvc;
using MvcBook.Data;
using MvcBook.Models;
using Microsoft.EntityFrameworkCore;


namespace MvcBook.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly MvcBookContext _context;
        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public ShoppingCartController(MvcBookContext context)
        {
            _context = context;
        }

        public ShoppingCartController GetCart(Controller controller, MvcBookContext context)
        {
            return new ShoppingCartController(context);
        }

        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            return View(cartItems);
        }

        public IActionResult AddToCart(int id)
        {
            var addedBook = _context.Books.Single(book => book.id == id);

            var cart = GetCart(this, _context);

            cart.AddToCartMethod(addedBook);

            return RedirectToAction("Index");
        }

        private void AddToCartMethod(Book book)
        {
            ShoppingCartId = GetCartId();
            // set cartItem where user id = ShoppingCartId and BookId = queried id
            var cartItem = _context.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.BookId == book.id);

            var contextBook = _context.Books.Single(b => b.id == book.id);
            // new iem for Book if not existing in cart
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    BookId = book.id,
                    CartId = ShoppingCartId,
                    Book = contextBook,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };
                _context.Add(cartItem);
            }
            // else update Book quantity
            else
            {
                cartItem.Quantity++;
            }
            _context.SaveChanges();
        }

        private string GetCartId()
        {
            /* try
             {
                 if (User.Identity != null)
                 {
                     HttpContext.Session.SetString(CartSessionKey, HttpContext.Session.Id.ToString());
                 }
                 else
                 { 
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Session.SetString(CartSessionKey, tempCartId.ToString());
                 }

             }8
             catch(Exception ex)
             {
                 Response.Redirect("Account/Logout");
             } */
            Guid tempCartId = Guid.NewGuid();
            return tempCartId.ToString();






        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();
            return _context.ShoppingCartItems
                .Where(c => c.CartId == ShoppingCartId)
                .Include(c => c.Book)
                .ToList();
        }
    }
}
