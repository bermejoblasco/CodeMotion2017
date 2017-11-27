namespace CodemtionRedisCache.Samples
{
    using CodemtionRedisCache.Connection;
    using CodemtionRedisCache.Models;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;

    /*
     http://redis.io/topics/data-types-intro#lists
     * What is it?
     * LinkedLists
     * Scenario: Add items to a long list in a fast manner in order.
     * Insert time constant at beginning or end
     * Lookup time is constant.
     * eg. last 5 tweets, inter process communication.
     * */

    public static class ListsUsage
    {
        public static string redisKey = "authors";
        public static List<Author> authors = Author.ListOfAuthors();

        public static void Run()
        {
            IDatabase cache = RedisConnection.Connection.GetDatabase();

            RevmoveValues(cache);

            Console.WriteLine("Add authors from rediskey with listlefpushasync");
            foreach (var item in authors)
            {
                cache.ListLeftPushAsync(ListsUsage.redisKey, item.Name + " " + item.Surname); // push the author onto the list 
                Console.WriteLine($"Author inserted: {item.Name} {item.Surname} ");
            }            

            // Accesf for the three last authors
            Console.WriteLine("Access the last 3 three authors");
            foreach (string author in cache.ListRange(ListsUsage.redisKey, 0, 2))
            {
                Console.WriteLine($"Authors retrvied: {author}");
            }

            RevmoveValues(cache);
        }

        private static void RevmoveValues(IDatabase cache)
        {
            cache.KeyDelete(ListsUsage.redisKey);
        }
    }
}
