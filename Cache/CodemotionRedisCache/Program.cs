namespace CodemtionRedisCache
{
    using CodemtionRedisCache.Samples;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("###### BASIC USAGE ######");
                BasicUsage.Run();
                ContinueExceution();

                Console.WriteLine("###### GENERAL USAGE ######");
                GeneralUsage.Run();
                ContinueExceution();

                Console.WriteLine("###### OBJECT USAGE ######");
                ObjectsUsage.Run();
                ContinueExceution();

                Console.WriteLine("###### LIST USAGE ######");
                ListsUsage.Run();
                ContinueExceution();

                Console.WriteLine("###### SORTED USAGE ######");
                SortedUsage.Run();

                Console.WriteLine("Finish OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Finish KO");
                Console.WriteLine($"Error: {ex.Message} - Exception: {ex}");
            }
            finally
            {
                Console.WriteLine("Press any key to finish");
                Console.ReadKey();
            }
        }

        private static void ContinueExceution()
        {
            Console.WriteLine("Press any key to Continue");
            Console.ReadKey();
        }
    }
}
