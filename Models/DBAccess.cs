using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksStore.Models
{
    public class DBAccess
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        public DBAccess()
        {
            _mongoClient = new MongoClient("mongodb://localhost:27017");
            _database = _mongoClient.GetDatabase("mDB");
        }
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public async Task<List<T>> GetAllAsync<T>(string collectionName)
        {
            var collection = GetCollection<T>(collectionName);
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task InsertAsync<T>(string collectionName, T document)
        {
            var collection = GetCollection<T>(collectionName);
            await collection.InsertOneAsync(document);
        }

        public async Task UpdateAsync<T>(string collectionName, string id, T updatedDocument)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
            await collection.ReplaceOneAsync(filter, updatedDocument);
        }
        public async Task DeleteAsync<T>(string collectionName, string id)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
            await collection.DeleteOneAsync(filter);
        }

        public async Task<long> GetCountAsync<T>(string collectionName)
        {
            // Define a filter to count specific documents (optional, use {} for all documents)
            var filter = Builders<T>.Filter.Empty; // Example: count all documents
            // Perform the count query
            long count = await GetCollection<T>(collectionName).CountDocumentsAsync(filter);

            return count;
        }

        // Add more methods as needed...
    }
}
