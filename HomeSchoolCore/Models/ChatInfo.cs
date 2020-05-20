using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolCore.Models
{
    public class ChatInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int messagesNumber { get; set; }
        public string subjectID { get; set; }
        public string classID { get; set; }

    }
}