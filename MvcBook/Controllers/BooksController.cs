using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        [AllowAnonymous]
        public IActionResult Index(string g, string q = null)
        {
            var books = _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .Where(b => b.Genres.Any(genre => genre.Name == g));

            if (!string.IsNullOrEmpty(q))
            {
                books = books.Where(b => b.Title.Contains(q) ||
                b.Authors.Any(a => a.Name.Contains(q)));
            }
            ViewBag.GenreName = g;
            return View(books);
        }


        [AllowAnonymous]
        public IActionResult Search(string q) 
        {
            IQueryable<Book> results = _context.Books.Include(b => b.Authors).Include(b => b.Genres).AsQueryable();

            if (!String.IsNullOrEmpty(q))
            {
                results = results
               .Where(b => b.Title.Contains(q) || b.Authors.Any(a => a.Name.Contains(q)));
            }

            results = results.OrderBy(b => b.Title).Take(10);
            ViewBag.Search = q;
            ViewBag.Count = results.Count();
            return View(results.ToList());
        }

        // GET: Books/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(m => m.id == id);
            book.Comments = _context.Comments.Where(c => c.BookId == id).ToList();

            foreach (var comment in book.Comments)
            {
                book.Rating += comment.Rating;
            }
            book.Rating /= (book.Comments.Count + 1);
            if (book == null)
            {
                return NotFound();
            }
            _context.SaveChangesAsync();
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Comment comment)
        {

            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            var intnewRating = dict["rating"];
            var commentText = dict["CommentText"];
            var bookId = dict["id_name"];
            var userName = dict["userName"];
            var intBookId = int.Parse(bookId);
            comment.UserId = userName;
            comment.CommentText = commentText;
            comment.Rating = int.Parse(intnewRating);
            comment.BookId = intBookId;
            comment.Book = _context.Books.SingleOrDefault(b => b.id == intBookId);


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
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }
            return View(comment.Book);

        }


        // GET: Books/Create
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", nameof(Author.Name));
            ViewData["Genres"] = new MultiSelectList(_context.Genres, "GenreId", nameof(Genre.Name));
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Title,Rating,Price,ImageUrl")] Book book, int[] AuthorIds, int[] GenreIds, string NewAuthorName)
        {
            var authorIdsList = AuthorIds.ToList();
            // create new author
            if (!string.IsNullOrEmpty(NewAuthorName))
            {
                var newAuthorNames = NewAuthorName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in newAuthorNames)
                {
                    var newAuthor = new Author { Name = name.Trim() };
                    _context.Authors.Add(newAuthor);
                    await _context.SaveChangesAsync();
                    authorIdsList.Add(newAuthor.AuthorId ?? 0);
                }
                AuthorIds = authorIdsList.ToArray();
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

                foreach (var authorId in AuthorIds)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null)
                    {
                        book.Authors.Add(author);
                    }
                }
                foreach (var genreId in GenreIds)
                {
                    var genre = await _context.Genres.FindAsync(genreId);
                    if (genre != null)
                    {
                        book.Genres.Add(genre);
                    }
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", nameof(Author.Name), AuthorIds);
            ViewData["Genres"] = new MultiSelectList(_context.Genres, "GenreId", nameof(Genre.Name), GenreIds);
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Policy = "AdminOnly")]
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
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", nameof(Author.Name));
            ViewData["Genres"] = new MultiSelectList(_context.Genres, "GenreId", nameof(Genre.Name));
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Title,Rating,Price,ImageUrl")] Book book, int[] AuthorIds, int[] GenreIds, string NewAuthorName)
        {
            if (id != book.id)
            {
                return NotFound();
            }

            ModelState.Remove("NewAuthorName");
            if (ModelState.IsValid)
            {
                try
                {
                    var authorIdsList = AuthorIds.ToList();
                    var genreIdsList = GenreIds.ToList();

                    var existingBook = await _context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .FirstOrDefaultAsync(b => b.id == id);

                    existingBook.Title = book.Title;
                    existingBook.Rating = book.Rating;
                    existingBook.Price = book.Price;
                    existingBook.ImageUrl = book.ImageUrl;

                    var existingAuthorIds = existingBook.Authors.Select(a => a.AuthorId).ToList();
                    var existingGenreIds = existingBook.Genres.Select(g => g.GenreId).ToList();

                    if (!string.IsNullOrEmpty(NewAuthorName))
                    {
                        var newAuthorNames = NewAuthorName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var name in newAuthorNames)
                        {
                            var newAuthor = new Author { Name = name.Trim() };
                            _context.Authors.Add(newAuthor);
                            await _context.SaveChangesAsync();
                            authorIdsList.Add(newAuthor.AuthorId ?? 0);
                        }
                        AuthorIds = authorIdsList.ToArray();
                    }

                    if (AuthorIds.Length > 0)
                    {
                        var authorsToAdd = AuthorIds.Where(a => !existingAuthorIds.Contains(a));
                        var authorsToRemove = existingAuthorIds.Where(e => !authorIdsList.Contains((int)e));

                        foreach (var authorId in authorsToAdd)
                        {
                            var author = await _context.Authors.FindAsync(authorId);
                            if (author != null)
                            {
                                existingBook.Authors.Add(author);
                            }
                        }
                        foreach (var authorId in authorsToRemove)
                        {
                            var author = existingBook.Authors.FirstOrDefault(a => a.AuthorId == authorId);
                            if (author != null)
                            {
                                existingBook.Authors.Remove(author);
                            }
                        }
                    }

                    if (GenreIds.Length > 0)
                    {
                        var genresToAdd = GenreIds.Where(a => !existingGenreIds.Contains(a));
                        var genresToRemove = existingGenreIds.Where(a => !genreIdsList.Contains((int)a));

                        foreach (var genreId in genresToAdd)
                        {
                            var genre = await _context.Genres.FindAsync(genreId);
                            if (genre != null)
                            {
                                existingBook.Genres.Add(genre);
                            }
                        }

                        foreach (var genreId in genresToRemove)
                        {
                            var genre = existingBook.Genres.FirstOrDefault(g => g.GenreId == genreId);
                            if (genre != null)
                            {
                                existingBook.Genres.Remove(genre);
                            }
                        }
                    }

                    _context.Update(existingBook);
                    _context.SaveChanges();
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
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "AuthorId", nameof(Author.Name), AuthorIds);
            ViewData["Genres"] = new MultiSelectList(_context.Genres, "GenreId", nameof(Genre.Name), GenreIds);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
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
