
namespace CodemtionRedisCache.Samples
{
    using CodemtionRedisCache.Connection;
    using StackExchange.Redis;
    using System;

    public static class BasicUsage
    {
        private const string key1 = "key1";
        private const string key2 = "key2";
        private const string key3 = "this is a key";

        public static void Run()
        {
            IDatabase cache = RedisConnection.Connection.GetDatabase();
                        
            // Remove values
            RevmoveValues(cache);

            //String SET
            Console.WriteLine("###### SET ######");

            cache.StringSet(BasicUsage.key1, 1);
            cache.StringSet(BasicUsage.key2, 2);
            cache.StringSet(BasicUsage.key3, 3);

            // String GET
            #region GET ASYNC

            Console.WriteLine("###### GET ASYNC ######");

            var taskKey1 = cache.StringGetAsync(BasicUsage.key1);
            var taskKey2 = cache.StringGetAsync(BasicUsage.key2);
            var taskKey3 = cache.StringGetAsync(BasicUsage.key3);

            var key1Value = cache.Wait(taskKey1);
            Console.WriteLine($"key1: {key1Value}");

            var key2Value = cache.Wait(taskKey2);
            Console.WriteLine($"key2: {key2Value}");

            var key3Value = cache.Wait(taskKey3);
            Console.WriteLine($"key3: {key3Value}");


            #endregion

            #region Increment

            Console.WriteLine("###### INCREMENT ######");
            int value = 50;

            cache.StringSet(BasicUsage.key1, value);
            Console.WriteLine("Key1 old value = " + value);
           
            long key1OldValue = cache.StringIncrement(BasicUsage.key1);
            Console.WriteLine("Key1 new value = " + key1OldValue);

            value = 0;
            string resultGetSet = cache.StringGetSet(BasicUsage.key1, value);
            Console.WriteLine("Key1 GETSET result = " + resultGetSet);
            Console.WriteLine("Key1 GET result= " + cache.StringGet(BasicUsage.key1));

            #endregion

            RevmoveValues(cache);
        }

        private static void RevmoveValues(IDatabase cache)
        {
            cache.KeyDelete(BasicUsage.key1);
            cache.KeyDelete(BasicUsage.key2);
            cache.KeyDelete(BasicUsage.key3);            
        }
    }

}
