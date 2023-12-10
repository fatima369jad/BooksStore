using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BooksStore.Models;
using MongoDB.Bson;

namespace BooksStore.Controllers
{
    public class AuthorController : Controller
    {
        private readonly DBAccess db = new DBAccess();


        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<Author> authors = new List<Author>();
            var reader = await db.GetAllAsync<Author>("Author");
            foreach (var res in reader)
            {
                Author author = new Author
                {
                    Id = res.Id,
                    FirstName = res.FirstName,
                    LastName = res.LastName,
                    DateOfBirth = Convert.ToDateTime(res.DateOfBirth)
                };

                authors.Add(author);
            }
            return View(authors);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            // Retrieve author details by ID
            var author = GetAuthorById(id);

            if (author == null)
            {
                // Handle the case where the author with the provided ID does not exist
                return HttpNotFound();
            }

            return View(await author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                // Check if the author with the provided ID exists
                if (await AuthorExists(author.Id))
                {
                    Author a = new Author { 
                        Id = author.Id,
                        FirstName = author.FirstName,
                        LastName = author.LastName,
                        DateOfBirth = author.DateOfBirth
                    };
                    await db.UpdateAsync<Author>("Author", author.Id, a);

                    // Redirect to the Index page after a successful update
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the case where the author with the provided ID does not exist
                    return HttpNotFound();
                }
            }

            // If ModelState is not valid, return the Edit view with validation errors
            return View(author);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            // Retrieve author details by ID
            var author = GetAuthorById(id);

            if (author == null)
            {
                // Handle the case where the author with the provided ID does not exist
                return HttpNotFound();
            }

            return View(await author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            // Check if the author with the provided ID exists
            await db.DeleteAsync<Author>("Author", id);
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
        public async Task<ActionResult> Add(Author author)
        {
            if (ModelState.IsValid)
            {

                await db.InsertAsync<Author>("Author", author);
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return the Add view with validation errors
            return View(author);
        }

        // Helper method to check if an author with a given ID exists
        private async Task<bool> AuthorExists(string id)
        {
            long count = await db.GetCountAsync<Author>("Author");
            return count > 0;
        }

        // Helper method to retrieve an author by their ID
        private async Task<Author> GetAuthorById(string id)
        {
            var reader = await db.GetAllAsync<Author>("Author");

            foreach (var res in reader)
            {
                Author author = new Author
                {
                    Id = res.Id,
                    FirstName = res.FirstName,
                    LastName = res.LastName,
                    DateOfBirth = Convert.ToDateTime(res.DateOfBirth)
                };

                return author;
            }
            return null;
        }
    }
}
