using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolCore.Models
{
    public class TextMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int messageID { get; set; }
        public string senderName { get; set; }
        public string senderSurname { get; set; }
        public string msessage { get; set; }
        public System.DateTime sendTime { get; set; }
    }
}