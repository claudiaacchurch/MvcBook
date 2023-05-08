using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcBook.Data;
using MvcBook.Models;

namespace MvcBook.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly MvcBookContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public ShoppingCartController(MvcBookContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            return View(cartItems);
        }

        [AllowAnonymous]
        public IActionResult AddToCart(int id)
        {
            var addedBook = _context.Books.Single(book => book.id == id);

            AddToCartMethod(addedBook);

            return RedirectToAction("Index");
        }

        private void AddToCartMethod(Book book)
        {
            ShoppingCartId = GetCartId();
            var cartItem = _context.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.BookId == book.id);

            var contextBook = _context.Books.Single(b => b.id == book.id);
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
            else
            {
                cartItem.Quantity++;
            }
            _context.SaveChanges();
        }
        public string GetCartId()
        {
            if (HttpContext.Session.GetString(CartSessionKey) == null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    HttpContext.Session.SetString(CartSessionKey, GetUserName());
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Session.SetString(CartSessionKey, tempCartId.ToString());
                }
            }
            return HttpContext.Session.GetString(CartSessionKey);
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();
            return _context.ShoppingCartItems
                .Where(c => c.CartId == ShoppingCartId)
                .Include(c => c.Book)
                .Include(c => c.Book.Authors)
                .ToList();
        }

        private string GetUserName()
        {
            return User.Identity.Name;
        }

        public void MigrateCart(string userName)
        {
            var shoppingCart = _context.ShoppingCartItems.Where(s => s.CartId == ShoppingCartId);
            foreach (CartItem item in shoppingCart)
            {
                item.CartId = userName;
            }
            _contextAccessor.HttpContext.Session.SetString(CartSessionKey, userName);
            _context.SaveChanges();
        }

        public async Task<ActionResult> DeleteCartItem(string id)
        {
            if (_context.ShoppingCartItems == null)
            {
                return Problem("Entity set 'MvcBookContext.ShoppingCartItems'  is null.");
            }
            if (id == null)
            {
                return Problem("Item is null.");
            }
            var cartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //get

        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([Bind("FirstName,LastName,Address,City,County,PostCode,Country,Phone,Email")] Order order)
        {

            order.OrderId = Guid.NewGuid().ToString();
            order.OrderDate = DateTime.Now;
            order.Username = GetUserName();
            order.PaymentTransactionId = "123";
            order.HasBeenShipped = false;
            order.CartId = GetCartId(); // Add the CartId property to the Order model
            order.ShoppingCartItems = _context.ShoppingCartItems.Where(a => a.CartId == order.CartId).Include(a => a.Book).ToList();
            order.Total = order.Total = (decimal)order.ShoppingCartItems.Sum(item => item.Quantity * item.Book.Price);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }
            else
            {
                if (order.OrderId != "")
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    // return View("ConfirmOrder", order);
                    return RedirectToAction("CreatePayment", "Payment", new { orderId = order.OrderId });
                }
                else
                {
                    // Show an error message if the OrderId is empty
                    ModelState.AddModelError(string.Empty, "Unable to process the order. Please try again.");
                }
            }
            return View(order);
        }
    }

}



