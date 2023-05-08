using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MvcBook.Data;
using MvcBook.Models;

namespace MvcBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MvcBookContext _context;

        public HomeController(ILogger<HomeController> logger, MvcBookContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var mvcBookContext = _context.Books.Include(b => b.Authors).Include(b => b.Genres);
            var sortedBooks = mvcBookContext.OrderByDescending(b => b.Rating).ToList();
            if (sortedBooks == null)
            {
                return View();
            }
            else
            {
                return View(sortedBooks);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}