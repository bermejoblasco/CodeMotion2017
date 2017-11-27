namespace GraphGetStarted
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.Azure.Graphs;
    using Microsoft.Azure.Graphs.Elements;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    public class Program
    {      
        public static void Main(string[] args)
        {
            try
            {
                string endpoint = ConfigurationManager.AppSettings["Endpoint"];
                string authKey = ConfigurationManager.AppSettings["AuthKey"];

                using (DocumentClient client = new DocumentClient(
                    new Uri(endpoint),
                    authKey,
                    new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
                {
                    Program p = new Program();
                    p.RunAsync(client).Wait();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error {ex}");
            }
        }
       
        public async Task RunAsync(DocumentClient client)
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "graphdb" });

            DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("graphdb"),
                new DocumentCollection { Id = "Persons" },
                new RequestOptions { OfferThroughput = 1000 });
          
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
            {
                { "Cleanup",        "g.V().drop()" },
                { "AddVertex 1",    "g.addV('employee').property('id', 'u001').property('firstName', 'John').property('age', 44)" },
                { "AddVertex 2",    "g.addV('employee').property('id', 'u002').property('firstName', 'Mary').property('age', 37)" },
                { "AddVertex 3",    "g.addV('employee').property('id', 'u003').property('firstName', 'Christie').property('age', 30)" },
                { "AddVertex 4",    "g.addV('employee').property('id', 'u004').property('firstName', 'Bob').property('age', 35)" },
                { "AddVertex 5",    "g.addV('employee').property('id', 'u005').property('firstName', 'Susan').property('age', 31)" },
                { "AddVertex 6",    "g.addV('employee').property('id', 'u006').property('firstName', 'Emily').property('age', 29)" },
                { "AddEdge 1",      "g.V('u002').addE('manager').to(g.V('u001'))" },
                { "AddEdge 2",      "g.V('u005').addE('manager').to(g.V('u001'))" },
                { "AddEdge 3",      "g.V('u004').addE('manager').to(g.V('u002'))" },
                { "AddEdge 4",      "g.V('u005').addE('friend').to(g.V('u006'))" },
                { "AddEdge 5",      "g.V('u005').addE('friend').to(g.V('u003'))" },
                { "AddEdge 6",      "g.V('u006').addE('friend').to(g.V('u003'))" },
                { "AddEdge 7",      "g.V('u006').addE('manager').to(g.V('u004'))" },
                { "ReturnVertex",   "g.V().hasLabel('employee').has('age', gt(40))" },
                { "AndOr",          "g.V().hasLabel('employee').and(has('age', gt(35)), has('age', lt(40)))" },
                { "Transversal",    "g.V('u002').out('manager').hasLabel('employee')" },
                { "outE/inV",       "g.V('u002').outE('manager').inV().hasLabel('employee')" },
                { "CountVertices",  "g.V().count()" },
                { "Filter Range",   "g.V().hasLabel('employee').and(has('age', gt(35)), has('age', lt(40)))" },
            };            

            foreach (KeyValuePair<string, string> gremlinQuery in gremlinQueries)
            {
                Console.WriteLine($"Running {gremlinQuery.Key}: {gremlinQuery.Value}");
              
                IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, gremlinQuery.Value);
                while (query.HasMoreResults)
                {
                    foreach (dynamic result in await query.ExecuteNextAsync())
                    {
                        Console.WriteLine($"\t {JsonConvert.SerializeObject(result)}");
                    }
                }

                Console.WriteLine();
            }

            //ReturnVertex operation
            await runQuery(client, graph, gremlinQueries, "ReturnVertex");            

            //AND/OR operation
            await runQuery(client, graph, gremlinQueries, "AndOr");

            //Transversal operation
            await runQuery(client, graph, gremlinQueries, "Transversal");

            //outE/inV operation
            await runQuery(client, graph, gremlinQueries, "outE/inV");

            //Filter Range operation
            await runQuery(client, graph, gremlinQueries, "Filter Range");


            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
        }

        private async Task runQuery(DocumentClient client, DocumentCollection graph, Dictionary<string, string> gremlinQueries, string operation)
        {
            string gremlin = gremlinQueries[operation];
            Console.WriteLine($"Running {operation} operation: {gremlin}");

            IDocumentQuery<Vertex> vertex = client.CreateGremlinQuery<Vertex>(graph, gremlin);
            while (vertex.HasMoreResults)
            {
                foreach (Vertex vertexResult in await vertex.ExecuteNextAsync<Vertex>())
                {                    
                    string name = (string)vertexResult.GetVertexProperties("firstName").First().Value;
                    Console.WriteLine($"\t Id:{vertexResult.Id}, Name: {name}");
                }
            }

            Console.WriteLine("Press key to continue");
            Console.ReadKey();
        }       
    }
}
