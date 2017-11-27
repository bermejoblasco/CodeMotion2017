
namespace CosmosDBTableApi
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.WindowsAzure.Storage.Table.Protocol;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    class Program
    {
        public static string tableName = "Magazines";

        static void Main(string[] args)
        {
            try
            {
                string connectionStringCosmosDB = "your comosdb connectionstring;";
                string connectionStringAzureTableStorage = "your table sotrage connectionsgring";
                
                int numIterations = 100;

                //Console.WriteLine("Start CosmosDB Table API \n");
                //CloudStorageAccount storageAccountCosmosDB = CloudStorageAccount.Parse(connectionStringCosmosDB);                
                //CloudTableClient tableClientCosmosDB = storageAccountCosmosDB.CreateCloudTableClient();
                
                Program program = new Program();

                //program.Run(tableClientCosmosDB, numIterations);

                //Console.WriteLine("************************** \n");
                Console.WriteLine("Start Azure Table Storage \n");
                CloudStorageAccount storageAccountAzureTableStorage = CloudStorageAccount.Parse(connectionStringAzureTableStorage);
                CloudTableClient tableClientAzureTableStorage = storageAccountAzureTableStorage.CreateCloudTableClient();

                program.Run(tableClientAzureTableStorage, numIterations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
        }

        public void Run(CloudTableClient tableClient, int numIterations)
        {
            List<ArticlesEntity> items = new List<ArticlesEntity>();
            Console.WriteLine("Creating Table if it doesn't exist...");
            //Create table
            CloudTable table = tableClient.GetTableReference(tableName);            
            table.DeleteIfExists();
            CreateTable(table);

            //Insert Elements
            Console.WriteLine("Insert elements ");
            InsertItemsInTable(numIterations, table, items);

            //Get Elements
            Console.WriteLine("Get elements: ");
            RetrieveElementsFromTable(numIterations, table, items);

            //Replace Elements
            Console.WriteLine("Replace elements: ");
            ReplaceElementsInTable(numIterations, table, items);

            //Delete Elements
            Console.WriteLine("Delete elements: ");
            DeleteElements(numIterations, table, items);

        }

        private void DeleteElements(int numIterations, CloudTable table, List<ArticlesEntity> items)
        {
            List<double> latencies = new List<double>();
            Stopwatch watch = new Stopwatch();
            for (int i = 0; i < numIterations; i++)
            {
                watch.Start();

                TableOperation deleteOperation = TableOperation.Delete(items[i]);
                table.Execute(deleteOperation);

                double latencyInMs = watch.Elapsed.TotalMilliseconds;
                Console.Write($"\r\tDelete #{i + 1} completed in {latencyInMs} ms");
                latencies.Add(latencyInMs);

                watch.Reset();
            }

            latencies.Sort();
            Console.WriteLine($"\n\tp0:{latencies[0]}, p50: {latencies[(int)(numIterations * 0.50)]}, p90: {latencies[(int)(numIterations * 0.90)]}");
        }

        private void ReplaceElementsInTable(int numIterations, CloudTable table, List<ArticlesEntity> items)
        {
            List<double> latencies = new List<double>();
            Stopwatch watch = new Stopwatch();

            for (int i = 0; i < numIterations; i++)
            {
                watch.Start();

                items[i].NumberOfArticles = 4;
                TableOperation replaceOperation = TableOperation.Replace(items[i]);
                table.Execute(replaceOperation);

                double latencyInMs = watch.Elapsed.TotalMilliseconds;
                Console.Write($"\r\tReplace #{i + 1} completed in {latencyInMs} ms");
                latencies.Add(latencyInMs);

                watch.Reset();
            }

            latencies.Sort();
            Console.WriteLine($"\n\tp0:{latencies[0]}, p50: {latencies[(int)(numIterations * 0.50)]}, p90: {latencies[(int)(numIterations * 0.90)]}");
        }

        private void RetrieveElementsFromTable(int numIterations, CloudTable table, List<ArticlesEntity> items)
        {
            List<double> latencies = new List<double>();
            Stopwatch watch = new Stopwatch();

            for (int i = 0; i < numIterations; i++)
            {
                watch.Start();
               
                TableOperation retrieveOperation = TableOperation.Retrieve<ArticlesEntity>(items[i].PartitionKey, items[i].RowKey);
                table.Execute(retrieveOperation);
                double latencyInMs = watch.Elapsed.TotalMilliseconds;

                Console.Write($"\r\tRetrieve #{i + 1} completed in {latencyInMs} ms");
                latencies.Add(latencyInMs);

                watch.Reset();
            }

            latencies.Sort();
            Console.WriteLine($"\n\tp0:{latencies[0]}, p50: {latencies[(int)(numIterations * 0.50)]}, p90: {latencies[(int)(numIterations * 0.90)]}");
        }

        private void InsertItemsInTable(int numIterations, CloudTable table, List<ArticlesEntity> items)
        {
            Stopwatch watch = new Stopwatch();
            List<double> latencies = new List<double>();

            for (int i = 0; i < numIterations; i++)
            {
                watch.Start();

                ArticlesEntity item = new ArticlesEntity()
                {
                    PartitionKey = Guid.NewGuid().ToString(),
                    RowKey = Guid.NewGuid().ToString(),
                    Email = $"{GetRandomString(6)}@mail.com",
                    LastArticleTitle = GetRandomString(50),
                    NumberOfArticles = GetRandomNumber(3)
                };

                // Insert item
                TableOperation insertOperation = TableOperation.Insert(item);
                table.Execute(insertOperation);
                double latencyInMs = watch.Elapsed.TotalMilliseconds;

                Console.Write($"\r\tInsert #{i + 1} completed in {latencyInMs} ms.");
                items.Add(item);
                latencies.Add(latencyInMs);

                watch.Reset();
            }

            latencies.Sort();
            Console.WriteLine($"\n\tp0:{latencies[0]}, p50: {latencies[(int)(numIterations * 0.50)]}, p90: {latencies[(int)(numIterations * 0.90)]}");
        }

        private bool CreateTable(CloudTable table)
        {
            do
            {
                try
                {                    
                    return table.CreateIfNotExists();
                }
                catch (StorageException e)
                {
                    if ((e.RequestInformation.HttpStatusCode == 409) && (e.RequestInformation.ExtendedErrorInformation.ErrorCode.Equals(TableErrorCodeStrings.TableBeingDeleted)))
                    {
                        Thread.Sleep(1000);
                    }// The table is currently being deleted. Try again until it works.
                    else
                    {
                        throw;
                    }
                }
            } while (true);
        }

        public string GetRandomString(int length)
        {
            Random random = new Random(System.Environment.TickCount);
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public int GetRandomNumber(int length)
        {
            Random random = new Random(System.Environment.TickCount);
            string chars = "123456789";
            return Convert.ToInt32(new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()));
        }

        public class ArticlesEntity : TableEntity
        {
            public ArticlesEntity(string authorLastName, string authorFirstName)
            {
                this.PartitionKey = authorLastName;
                this.RowKey = authorFirstName;
            }

            public ArticlesEntity() { }

            public string Email { get; set; }

            public string LastArticleTitle { get; set; }

            public int NumberOfArticles { get; set; }
        }
    }
}
