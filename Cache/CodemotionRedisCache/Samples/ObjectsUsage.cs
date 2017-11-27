
namespace CodemtionRedisCache.Samples
{
    using CodemtionRedisCache.Connection;
    using CodemtionRedisCache.Models;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Linq;

    public static class ObjectsUsage
    {
        private const string booksKey1 = "booksKey1";

        public static void Run()
        {
            IDatabase cache = RedisConnection.Connection.GetDatabase();

            #region JSON

            Console.WriteLine("###### JSON ######");
            // Remove values
            RevmoveValues(cache);

            var book = CreateBook();
            Console.WriteLine("Inserted Book");
            cache.StringSet(ObjectsUsage.booksKey1, JsonConvert.SerializeObject(book));

            Console.WriteLine("Get Book");
            book = JsonConvert.DeserializeObject<Book>(cache.StringGet(ObjectsUsage.booksKey1));
            Console.WriteLine($"ISVN: {book.ISBN}");
            Console.WriteLine($"Tittle: {book.Title}");
            Console.WriteLine($"Author Name: {book.Authors.First().Name} {book.Authors.First().Surname}");

            RevmoveValues(cache);

            #endregion
        }

        private static void RevmoveValues(IDatabase cache)
        {
            cache.KeyDelete(ObjectsUsage.booksKey1);
        }

        private static Book CreateBook()
        {
            return new Book()
            {
                ISBN = "0132350882 ",
                Title = "Clean Code",
                Authors = Author.ListOfAuthors().Where(x => x.Name.Equals("Robert")).ToList()
             };
        }
    }
}

