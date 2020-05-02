using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolAPI.Models
{
    public class Response
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string homeworkID { get; set; }
        public string description { get; set; }
        public List<string> responses { get; set; }
        public DateTime sendTime { get; set; }
    }
}