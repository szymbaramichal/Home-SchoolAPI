using System.Collections.Generic;
using HomeSchoolCore.APIRequest;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolCore.Models
{
    public class QuizQuestion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string QuizId { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}