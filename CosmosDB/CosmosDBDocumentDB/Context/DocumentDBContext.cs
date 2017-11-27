namespace CosmosDBDocumentDB.Context
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using CosmosDBDocumentDB.Data;
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    public static class DocumentDBContext
    {
        private static DocumentClient client;
        private static readonly string endpointUrl = "yourcosmosendpoint";
        private static readonly string authorizationKey = "yourkey";

        public static void CreateClient()
        {
            client =  new DocumentClient(new Uri(endpointUrl), authorizationKey, new ConnectionPolicy { ConnectionMode = ConnectionMode.Gateway, ConnectionProtocol = Protocol.Https });
        }

        public static DocumentClient GetClient()
        {
            CreateClienteIfNoExit();
            return client;
        }

        public static async Task<Database> CreateDatabaseIfNotExistAsync(string databaseName)
        {
            CreateClienteIfNoExit();
            return await client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });
        }

        public static async Task<DocumentCollection> CreateCollection(string databaseName, string collection)
        {
            DocumentCollection collectionDefinition = new DocumentCollection();
            collectionDefinition.Id = collection;
            //collectionDefinition.PartitionKey.Paths.Add("/LastName");

            return await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseName),
                collectionDefinition,
                new RequestOptions { OfferThroughput = 400 });
        }

        public static Uri CreateDocumentCollectionUri(string databaseName, string collection)
        {
            return UriFactory.CreateDocumentCollectionUri(databaseName, collection);
        }

        public static async Task InicializeCollection(string databaseName, string collection)
        {
            var AndersenFamily = FamilyBuilder.GetFamily("AndersenFamily");
            await client.UpsertDocumentAsync(CreateDocumentCollectionUri(databaseName,collection), AndersenFamily);
            var WakefieldFamily = FamilyBuilder.GetFamily("WakefieldFamily");
            await client.UpsertDocumentAsync(CreateDocumentCollectionUri(databaseName, collection), WakefieldFamily);
        }

        private static void CreateClienteIfNoExit()
        {
            if (client != null) { return; }
            CreateClient();
        }
    }
}
