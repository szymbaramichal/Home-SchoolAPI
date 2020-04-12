using System.Collections.Generic;
using HomeSchoolAPI.APIRespond;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public string surrname { get; set; }
        public string email { get; set; }
        public int userRole { get; set; }
        public List<string> friends { get; set; }
        public string username { get; set; }
        public string userCode { get; set; } //0 -> student
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}