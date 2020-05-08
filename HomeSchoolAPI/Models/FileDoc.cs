using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolAPI.Models
{
    public class FileDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public byte[] fileContent { get; set; } 
        public string senderID { get; set; }
        public string contentType { get; set; }
    }
}