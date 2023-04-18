﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index()
        {
            var mvcBookContext = _context.Books.Include(b => b.Author).Include(b => b.Genre);
            var sortedBooks = mvcBookContext.OrderByDescending(b => b.Rating).ToList();
            return View(sortedBooks);
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