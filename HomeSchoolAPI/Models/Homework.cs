using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolAPI.Models
{
    public class Homework
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string subjectID { get; set; }
        public List<string> responses { get; set; }
        public List<IFormFile> files { get; set; }
    }
}