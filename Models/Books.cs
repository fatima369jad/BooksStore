using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksStore.Models
{
    public class Books
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string ISBN13 { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AuthorID { get; set; } // You might change this to ObjectId if it references Author's Id
    }
}