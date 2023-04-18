using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MvcBook.Data;
using MvcBook.Models;

namespace MvcBook.Controllers
{
    public class BooksController : Controller
    {
        private readonly MvcBookContext _context;

        public BooksController(MvcBookContext context)
        {
            _context = context;
        }

        // GET: Books
        public IActionResult Index(string q)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set Books is null");
            }

            IQueryable<Book> results = _context.Books.Include(b => b.Author).Include(b => b.Genre).AsQueryable();
            
            if (!String.IsNullOrEmpty(q))
            {
                results = results
               .Where(a => a.Title.Contains(q) || a.Author.Name.Contains(q))
               .Take(10);
            }

            results = results.OrderBy(b => b.Title);
            return View(results.ToList());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", nameof(Author.Name));
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", nameof(Genre.Name));
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Title,Rating,Price,ImageUrl,AuthorId,GenreId")] Book book, string NewAuthorName)
        {
            // create new author
            if (!string.IsNullOrEmpty(NewAuthorName))
            {
                var newAuthor = new Author { Name = NewAuthorName };
                _context.Authors.Add(newAuthor);
                await _context.SaveChangesAsync();
                book.AuthorId = (int)newAuthor.AuthorId;
            }

            ModelState.Remove("NewAuthorName");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "AuthorId", nameof(Author.Name), book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", nameof(Genre.Name), book.GenreId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", nameof(Author.Name));
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", nameof(Genre.Name));
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Title,Rating,Price,ImageUrl,AuthorId,GenreId")] Book book)
        {
            if (id != book.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "AuthorId", "AuthorId", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", "GenreId", book.GenreId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'MvcBookContext.Book'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
