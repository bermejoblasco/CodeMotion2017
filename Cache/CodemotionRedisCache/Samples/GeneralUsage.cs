
namespace CodemtionRedisCache.Samples
{
    using CodemtionRedisCache.Connection;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public static class GeneralUsage
    {
        private const string key1 = "key1";
        private const string key2 = "key2";
        private const string key3 = "this is a key";

        public static void Run()
        {
            IDatabase cache = RedisConnection.Connection.GetDatabase();
                        
            // Remove values
            RevmoveValues(cache);

            #region SET/ GET LISTS

            Console.WriteLine("######M SET/ GET LISTS ######");
            var keysAndValues =
                new List<KeyValuePair<RedisKey, RedisValue>>()
                    {
                        new KeyValuePair<RedisKey, RedisValue>(GeneralUsage.key1, "valueKey1"),
                        new KeyValuePair<RedisKey, RedisValue>(GeneralUsage.key2, "valueKey2"),
                        new KeyValuePair<RedisKey, RedisValue>(GeneralUsage.key3, "valueKey3")
                    };

            // Store the list of key/value pairs in the cache
            cache.StringSet(keysAndValues.ToArray());
            Console.WriteLine("Set List in Redis");

            // Find all values that match a list of keys
            RedisKey[] keys = { GeneralUsage.key1, GeneralUsage.key2, GeneralUsage.key3 };

            Console.WriteLine("Get all List keys");
            RedisValue[] valuesKeys = cache.StringGet(keys);
            
            foreach (var item in valuesKeys)
            {
                Console.WriteLine(item);
            }

            // Find all values that match a list of keys
            keys = new RedisKey[] { GeneralUsage.key1, GeneralUsage.key3 };

            Console.WriteLine("Get first and last keys");
            valuesKeys = cache.StringGet(keys);

            foreach (var item in valuesKeys)
            {
                Console.WriteLine(item);
            }

            #endregion

            #region Expiration

            RevmoveValues(cache);
            // Key Expiration
            cache.StringSet(GeneralUsage.key1, 1, TimeSpan.FromSeconds(1));
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine($"Key has expired. Value = {cache.StringGet(GeneralUsage.key1)}");

            #endregion

            #region Transaction

            RevmoveValues(cache);

            Console.WriteLine("###### Transaction ######");

            ITransaction transaction = cache.CreateTransaction();
            
            var keyValue1 = transaction.StringIncrementAsync(GeneralUsage.key1);
            var keyValue2 = transaction.StringDecrementAsync(GeneralUsage.key2);

            bool result = transaction.Execute();
            
            Console.WriteLine("Transaction {0}", result ? "succeeded" : "failed");
            Console.WriteLine($"Result of increment: {keyValue1.Result}");
            Console.WriteLine($"Result of decrement: {keyValue2.Result}");

            #endregion

            RevmoveValues(cache);
        }

        private static void RevmoveValues(IDatabase cache)
        {
            cache.KeyDelete(GeneralUsage.key1);
            cache.KeyDelete(GeneralUsage.key2);
            cache.KeyDelete(GeneralUsage.key3);            
        }
    }

}
