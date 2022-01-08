using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Catalog.Api.Entities
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal Price { get; set; }
        [BsonIgnore] //ignore this value in MongoDB
        public JObject RequestParams { get; set; }

        [JsonIgnore] //ignore this value in the response on Get requests
        [BsonElement(elementName: "requestParams")]
        public BsonDocument RequestParamsJson { get; set; }

    }
}