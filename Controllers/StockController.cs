using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using BooksStore.Models;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BooksStore.Controllers
{
    public class StockController : Controller
    {
        private readonly DBAccess db = new DBAccess();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<StockSaldo> stockItems = new List<StockSaldo>();

            var reader = await db.GetAllAsync<StockSaldo>("StockSaldo");

            foreach (var res in reader)
            {
                StockSaldo stockItem = new StockSaldo
                {
                    StoreID = res.StoreID,
                    ISBN = res.ISBN,
                    Number = res.Number,
                };

                stockItems.Add(stockItem);
            }

            return View(stockItems);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string storeId, string isbn)
        {
            var stockItem = GetStockItem(storeId, isbn);

            if (stockItem == null)
            {
                return HttpNotFound();
            }

            return View(await stockItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StockSaldo stockItem)
        {
            if (ModelState.IsValid)
            {
                if (await StockItemExists(stockItem.StoreID, stockItem.ISBN))
                {
                    var filter = Builders<StockSaldo>.Filter.Eq(s => s.StoreID, stockItem.StoreID) & Builders<StockSaldo>.Filter.Eq(s => s.ISBN, stockItem.ISBN);

                    var update = Builders<StockSaldo>.Update.Set(s => s.Number, stockItem.Number);

                    var updateResult = await db.GetCollection<StockSaldo>("StockSaldo").UpdateOneAsync(filter, update);

                    return RedirectToAction("Index");
                }
                else
                {
                    
                    return HttpNotFound();
                }
            }

            return View(stockItem);
        }

        private async Task<bool> StockItemExists(string storeId, string isbn)
        {
            var filter = Builders<StockSaldo>.Filter.Eq(s => s.StoreID, storeId) & Builders<StockSaldo>.Filter.Eq(s => s.ISBN, isbn);

            long count = await db.GetCollection<StockSaldo>("StockSaldo").CountDocumentsAsync(filter);

            return count > 0;
            
        }

        private async Task<StockSaldo> GetStockItem(string storeId, string isbn)
        {

            var filter = Builders<StockSaldo>.Filter.Eq(s => s.StoreID, storeId) &
             Builders<StockSaldo>.Filter.Eq(s => s.ISBN, isbn);

            var result = await db.GetCollection<StockSaldo>("StockSaldo").Find(filter).ToListAsync();

            foreach (var res in result)
            {
                StockSaldo stockItem = new StockSaldo
                {
                    StoreID = res.StoreID,
                    ISBN = res.ISBN,
                    Number = res.Number
                };


                return stockItem;
            }
            return null;
        }

        // ... (previous code)

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(StockSaldo stockItem)
        {
            if (ModelState.IsValid)
            { 
                await db.InsertAsync<StockSaldo>("StockSaldo", stockItem);

                return RedirectToAction("Index");
              
            }

            return View(stockItem);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string storeId, string isbn)
        {
            var stockItem = GetStockItem(storeId, isbn);

            if (stockItem == null)
            {
                return HttpNotFound();
            }

            return View(await stockItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string storeId, string isbn)
        {
            if (await StockItemExists(storeId, isbn))
            {
                var filter = Builders<StockSaldo>.Filter.Eq(s => s.StoreID, storeId) &
             Builders<StockSaldo>.Filter.Eq(s => s.ISBN, isbn);
                var result = await db.GetCollection<StockSaldo>("StockSaldo").DeleteOneAsync(filter);

                return RedirectToAction("Index");
            }
            else
            {
                
                return HttpNotFound();
            }
        }

        // ... (remaining code)

    }
}
