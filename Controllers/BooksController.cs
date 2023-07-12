using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using CatalogApp.Models;
using CatalogApp.Data;


namespace CatalogApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly catalogContext _context;
        public readonly string[] _extensions = {".txt", ".epub", ".pdf", ".bibtex"};
        public BooksController(catalogContext context) {_context = context;}

        public async Task<IActionResult> Index(string sortBy, string filter, string searchString, int? page)
        {   
            if (_context.Book == null)
                {
                    return Problem("Entity Set is null");
                }
            
            ViewData["SortedBy"] = sortBy;
            ViewData["TitleSort"] = string.IsNullOrEmpty(sortBy) ? "title_desc" : "";
            ViewData["AuthorSort"] = sortBy == "Author" ? "author_desc" : "Author";

            if (searchString != null)
            {   
                page = 1;
            }
            else 
            {
                searchString = filter;
            }

            var books = from b in _context.Book
                select b;

            if(!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title!.ToLower().Contains(searchString.ToLower()) 
                || b.Author!.ToLower().Contains(searchString.ToLower())
                || b.Series!.ToLower().Contains(searchString.ToLower())); 
            }

            ViewData["Filter"] = searchString;

            books = sortBy switch
            {
                "Author" => books.OrderBy(b => b.Author),
                "author_desc" => books.OrderByDescending(b => b.Author),
                "title_desc" => books.OrderByDescending(b => b.Title),
                _=> books.OrderBy(b => b.Title)
            };

            int pageSize = 5;
            return View(await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), page ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,Restrict,AgeGroup,Series,Publisher,Isbn,Location,BookFormFile")] BookFile bookFile)
        {
            if (ModelState.IsValid)
            {
                var book = new Book();
                book = ConvertFile(bookFile.BookFormFile, book);
                book.Title = bookFile.Title;
                book.Author = bookFile.Author;
                book.Genre = bookFile.Genre;
                book.Restrict = bookFile.Restrict;
                book.AgeGroup = bookFile.AgeGroup;
                book.Series = bookFile.Series;
                book.Publisher = bookFile.Publisher;
                book.Isbn = bookFile.Isbn;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookFile);
        }

        // To-Do: Add security - virus scan, signature check, allowed file types, etc.
        // To-Do: Add large file upload support
        private Book ConvertFile(IFormFile bookFormFile, Book book)
        {
            if (bookFormFile != null)
            {                       
                using (var memoryStream = new MemoryStream())
                {
                    bookFormFile.CopyTo(memoryStream);
                    book.BookArray = memoryStream.ToArray();
                    book.FileExtension = Path.GetExtension(bookFormFile.FileName);
                }
            }
            else
            {
                book.Location = "No File Uploaded";
            }
            return book;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,Restrict,AgeGroup,Series,Publisher,Isbn,Location")] Book book)
        {
            if (id != book.Id)
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
                    if (!BookExists(book.Id))
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

            return View(book);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'catalogContext.Book' is null.");
            }

            var book = await _context.Book.FindAsync(id);
            
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Download(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = _context.Book.FirstOrDefault(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            if (book.BookArray == null)
            {
                return NotFound();
            }

            return File(book.BookArray, "application/octet-stream", book.Title + book.FileExtension);
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
