
namespace CosmosDBDocumentDB.Model
{
    using Newtonsoft.Json;
    using System;

    internal sealed class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string LastName { get; set; }

        public Parent[] Parents { get; set; }

        public Child[] Children { get; set; }

        public Address Address { get; set; }

        public bool IsRegistered { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
