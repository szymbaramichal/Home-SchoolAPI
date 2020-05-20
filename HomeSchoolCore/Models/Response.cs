using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolCore.Models
{
    public class Response
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string homeworkID { get; set; }
        public string senderID { get; set; }
        public string senderName { get; set; }
        public string senderSurname { get; set; }
        public string homeworkName { get; set; }
        public string mark { get; set; }
        public string description { get; set; }
        public List<string> files { get; set; }
        public DateTime sendTime { get; set; }
        public List<string> linkHrefs { get; set; }
    }
}