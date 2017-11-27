
namespace CodemtionRedisCache.Samples
{
    using CodemtionRedisCache.Connection;
    using CodemtionRedisCache.Models;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;

    /*
     * Leaderboard scenarios.
     * http://redis.io/topics/data-types-intro#sorted-sets
     *  
     */

    public static class SortedUsage
    {
        public static string redisKey = "booksScore";
        public static List<Book> books = Book.ListOfBooks();

        public static void Run()
        {
            IDatabase cache = RedisConnection.Connection.GetDatabase();

            Console.WriteLine("###### SORT ######");

            RevmoveValues(cache);
            Console.WriteLine("Insert Books");
            // Add books
            foreach (var book in books)
            {
                cache.SortedSetAdd(SortedUsage.redisKey, book.Title, book.Score);
                Console.WriteLine($"Book inserted: {book.Title} - {book.Score}");
            }

            Console.WriteLine("Get Books SortedSetRangeByRankWithScores");
            foreach (var book in cache.SortedSetRangeByRankWithScores(SortedUsage.redisKey))
            {
                Console.WriteLine(book);
            }

            Console.WriteLine("Get SortedSetRangeByRankWithScores Descending");
            foreach (var book in cache.SortedSetRangeByRankWithScores(
                               SortedUsage.redisKey, order: Order.Descending))
            {
                Console.WriteLine($"Book retrived: {book}");
            }

            Console.WriteLine("Books with scores between 8 and 10");
            foreach (var book in cache.SortedSetRangeByScoreWithScores(
                                           redisKey, 8,10))
            {
                Console.WriteLine($"Book retrived: {book}");
            }

            //RevmoveValues(cache);
        }

        private static void RevmoveValues(IDatabase cache)
        {
            cache.KeyDelete(SortedUsage.redisKey);
        }     
    }
}
