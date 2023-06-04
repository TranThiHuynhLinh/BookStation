using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.Data;
using Microsoft.AspNetCore.Authorization;


namespace BookStation.Controllers
{
    [Authorize(Roles = "admin")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private String getRamdom()
        {
            Random rand = new Random();
            int randomNumber = rand.Next();
           return randomNumber.ToString();
        }

        private String getExtensionFile(IFormFile formFile)
        {
            int lastDotIndex = formFile.FileName.LastIndexOf('.');
            return (lastDotIndex >= 0 ? formFile.FileName.Substring(lastDotIndex) : string.Empty);
        }

        private async Task<String> saveBookImage(int bookId, IFormFile formFile, String pathBookImg)
        {
            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
            //var savedFileName = bookId + "_" + getRamdom() + getExtensionFile(formFile);
            var savedFileName = bookId.ToString() + getExtensionFile(formFile);
            var finalFilePath = Path.Combine(pathBookImg, savedFileName);

            System.IO.File.Move(tempFilePath, finalFilePath);
            return savedFileName;
        }

        private void deleteBookImage(String path, String filePath)
        {
            string fullPath = Path.Combine(path, filePath);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
              return _context.Books != null ? 
                          View(await _context.Books.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Books'  is null.");
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            IEnumerable<Category> objCategoryList = _context.Categories;
            IEnumerable<Author> objAuthorList = _context.Authors;
            IEnumerable<Book> objBookList = _context.Books;
            var viewModel = new AuthorCategory
            {
                Categories = objCategoryList,
                Authors = objAuthorList,
                Books = objBookList,
            };
            return View(viewModel);
            //return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,BookName,AuthorId,CategoryId1,CategoryId2,CategoryId3,Summary,FileUpload")] Book book)
        {
            if (ModelState.IsValid)
            {
                // Save book without Path and ImageName
                _context.Add(book);
                await _context.SaveChangesAsync();

                // Save image file
                if (book?.FileUpload != null && book?.FileUpload.Length != 0)
                {

                    var pathBookImg = _webHostEnvironment.WebRootPath + "/img/book_img";
                    var savedFileName = saveBookImage(book.BookId, book.FileUpload, pathBookImg);

                    // Update Path and ImageName
                    book.ImageName = savedFileName.Result;
                    book.Path = pathBookImg;
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("No file selected for upload.");
                }

                return RedirectToAction(nameof(Index));
            }

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
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,BookName,AuthorId,CategoryId1,CategoryId2,CategoryId3,Summary,FileUpload")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var bookOldData = _context.Books.AsNoTracking().FirstOrDefault(e => e.BookId == id);
                    if (book?.FileUpload != null)
                    {
                        // User changes Image for book
                        var pathBookImg = _webHostEnvironment.WebRootPath + "/img/book_img";
                        // 1. Delete file on directory
                        if (bookOldData?.ImageName != null)
                        {
                            deleteBookImage(pathBookImg, bookOldData?.ImageName);
                        }
                        // 2. Save new image
                        var savedFileName = await saveBookImage(id, book.FileUpload, pathBookImg);
                        book.ImageName = savedFileName;
                        book.Path = pathBookImg;
                    } else
                    {
                        // If user does not change image, we re-use old ImageName and Path
                        book.ImageName = bookOldData?.ImageName;
                        book.Path = bookOldData?.Path;
                    }
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
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
                return Problem("Entity set 'ApplicationDbContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            string imageName = book.ImageName;
            //var favouriteBook = await _context.FavouriteBooks.FirstOrDefaultAsync(b => b.BookId == id);
            var booksToRemove = await _context.FavouriteBooks.Where(b => b.BookId == id).ToListAsync();
            var reviewToRemove = await _context.Reviews.Where(r => r.BookId == id).ToListAsync();
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                var pathBookImg = _webHostEnvironment.WebRootPath + "/img/book_img";
                // 1. Delete file on directory
                if (book.ImageName != null)
                {
                    deleteBookImage(pathBookImg, book.ImageName);
                }

            }
            if (booksToRemove != null)
            {
                _context.FavouriteBooks.RemoveRange(booksToRemove);
                await _context.SaveChangesAsync();
            }
            if(reviewToRemove != null)
            {
                _context.Reviews.RemoveRange(reviewToRemove);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
