using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolAPI.Models
{
    public class Class
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string creatorID { get; set; }
        public string className { get; set; }
        public string schoolName { get; set; }
        public int membersAmount { get; set; }
        public List<string> members { get; set; }
        public List<string> subjects { get; set; }
    }
}