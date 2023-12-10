using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;
using BooksStore.Models;
using System.Threading.Tasks;

namespace BooksStore.Controllers
{
    public class BookController : Controller
    {
        private readonly DBAccess db = new DBAccess();

        // GET: Book

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<Books> books = new List<Books>();
            var reader = await db.GetAllAsync<Books>("Books");
            foreach (var res in reader)
            {
                Books book = new Books
                {
                    ISBN13 = res.ISBN13,
                    Title = res.Title,
                    Language = res.Language,
                    Price = Convert.ToDecimal(res.Price),
                    ReleaseDate = Convert.ToDateTime(res.ReleaseDate),
                    AuthorID = res.AuthorID
                };

                books.Add(book);
            }

            return View(books);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            // Retrieve book details by ISBN
            var book = GetBookById(id);

            if (book == null)
            {
                // Handle the case where the book with the provided ISBN does not exist
                return HttpNotFound();
            }

            return View(await book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute for security
        public async Task<ActionResult> Edit(Books book)
        {
            if (ModelState.IsValid)
            {
                // Validate the ModelState before proceeding with the update

                // Check if the book with the provided ISBN exists
                if (await BookExists(book.Id))
                {
                    Books b = new Books { 
                        Id = book.Id,
                        Title = book.Title,
                        Language = book.Language,
                        Price = book.Price,
                        ReleaseDate = book.ReleaseDate,
                        AuthorID = book.AuthorID
                    };

                    await db.UpdateAsync<Books>("Books", book.Id, b);
                    // Redirect to the Details page after a successful update
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the case where the book with the provided ISBN does not exist
                   
                    return HttpNotFound();
                }
            }

            // If ModelState is not valid, return the Edit view with validation errors
            return View(book);
        }

        // Helper method to check if a book with a given ISBN exists
        private async Task<bool> BookExists(string id)
        {
            long count = await db.GetCountAsync<Books>("Books");
            return count > 0;
           
        }

        // Helper method to retrieve a book by its ISBN
        private async Task<Books> GetBookById(string ISBN)
        {

            var reader = await db.GetAllAsync<Books>("Books");

            foreach (var res in reader)
            {
                Books book = new Books
                {
                    Id = res.Id,
                    ISBN13 = res.ISBN13,
                    Title = res.Title,
                    Language = res.Language,
                    Price = Convert.ToDecimal(res.Price),
                    ReleaseDate = Convert.ToDateTime(res.ReleaseDate),
                    AuthorID = res.AuthorID
                };

                return book;
            }

            return null;
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            // Retrieve book details by ISBN
            var book = GetBookById(id);

            if (book == null)
            {
                // Handle the case where the book with the provided ISBN does not exist
                return HttpNotFound();
            }

            return View(await book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            // Check if the book with the provided ISBN exists

            if (await BookExists(id)) 
            {      
                await db.DeleteAsync<Books>("Books", id);
            }

            // Redirect to the Index page after a successful delete
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Books book)
        {
            if (ModelState.IsValid)
            {
                await db.InsertAsync<Books>("Books", book);
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return the Add view with validation errors
            return View(book);
        }


    }
}
