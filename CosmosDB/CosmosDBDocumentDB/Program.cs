
namespace CosmosDBDocumentDB
{
    using Microsoft.Azure.Documents.Client;
    using CosmosDBDocumentDB.Context;
    using CosmosDBDocumentDB.Model;
    using System;
    using System.Threading.Tasks;
    using System.Linq;

    class Program
    {
        private static DocumentClient client;
        private static Uri collectionUri;
        private static readonly string DatabaseName = "Codemotion2017NoSQL";
        private static readonly string CollectionName = "Codemotion2017Collection";
        private static readonly FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        static void Main(string[] args)
        {
            try
            {
                RunAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();

            }
        }

        private static async Task RunAsync()
        {
            Console.WriteLine("Start Create Database");
            var documentDB = await DocumentDBContext.CreateDatabaseIfNotExistAsync(DatabaseName);
            Console.WriteLine("End Create Database");

            Console.WriteLine("Start Create Collection");
            var collection = await DocumentDBContext.CreateCollection(DatabaseName, CollectionName);
            Console.WriteLine("End Create Collection");

            client = DocumentDBContext.GetClient();
            collectionUri = DocumentDBContext.CreateDocumentCollectionUri(DatabaseName, CollectionName);

            Console.WriteLine("Start Inicialize Collection");
            await DocumentDBContext.InicializeCollection(DatabaseName, CollectionName);
            Console.WriteLine("End Inicialize Collection");

            ContinueProcess();

            Console.WriteLine("Start Get All documents from collection");
            QueryAllDocuments();
            Console.WriteLine("End Get All documents from collection");

            ContinueProcess();

            Console.WriteLine("Start Query wiht Joins");
            QueryWithJoins();
            Console.WriteLine("End Query wiht Joins");

            ContinueProcess();

            Console.WriteLine("Start Query wiht Joins And filter");
            QueryWithTwoJoinsAndFilter();
            Console.WriteLine("End Query wiht Joins And filter");            
        }

        private static void QueryAllDocuments()
        {
            // LINQ
            var families = client.CreateDocumentQuery<Family>(collectionUri);
            Console.WriteLine($"LinQ all documents. Number of documents: {families.ToList().Count}");
            // SQL
            var familiesSQL = client.CreateDocumentQuery<Family>(collectionUri, "SELECT * FROM Families", DefaultOptions);
            Console.WriteLine($"SQL all documents. Number of documents: {familiesSQL.ToList().Count}");
        }

        private static void QueryWithJoins()
        {
            // LINQ
            var familiesChildrenAndPets = client.CreateDocumentQuery<Family>(collectionUri, DefaultOptions)
                    .SelectMany(family => family.Children
                    .SelectMany(child => child.Pets
                    .Select(pet => new
                    {
                        family = family.Id,
                        child = child.FirstName,
                        pet = pet.GivenName
                    }
                    )));

            Console.WriteLine("LinQ Results");
            foreach (var item in familiesChildrenAndPets)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("SQL Results");
            var familiesChildrenAndPetsSQL = client.CreateDocumentQuery<dynamic>(
                    collectionUri,
                  "SELECT f.id as family, c.FirstName AS child, p.GivenName AS pet " +
                  "FROM Families f " +
                  "JOIN c IN f.Children " +
                  "JOIN p IN c.Pets ");

            foreach (var item in familiesChildrenAndPetsSQL)
            {
                Console.WriteLine(item);
            }           
        }

        private static void QueryWithTwoJoinsAndFilter()
        {         
            // LINQ
            var familiesChildrenAndPets = client.CreateDocumentQuery<Family>(collectionUri, DefaultOptions)
                    .SelectMany(family => family.Children
                    .SelectMany(child => child.Pets
                    .Where(pet => pet.GivenName == "Fluffy")
                    .Select(pet => new
                    {
                        family = family.Id,
                        child = child.FirstName,
                        pet = pet.GivenName
                    }
                    )));

            Console.WriteLine("LinQ Results");
            foreach (var pet in familiesChildrenAndPets)
            {
                Console.WriteLine(pet);
            }

            //SQL
            var query = client.CreateDocumentQuery<dynamic>(collectionUri,
                   "SELECT f.id as family, c.FirstName AS child, p.GivenName AS pet " +
                   "FROM Families f " +
                   "JOIN c IN f.Children " +
                   "JOIN p IN c.Pets " +
                   "WHERE p.GivenName = 'Fluffy'",
                   DefaultOptions);

            //var items = query.ToList();
            Console.WriteLine("SQL Results");
            foreach (var item in query)
            {
                Console.WriteLine(item);
            }
        }

        private static void ContinueProcess()
        {
            Console.WriteLine("Press Key to continue with process");
            Console.ReadKey();
        }
    }
}
