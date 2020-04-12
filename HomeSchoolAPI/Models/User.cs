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
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int userRole { get; set; } //0 -> student
        public string userCode { get; set; } // ID of class
        public List<string> friends { get; set; }
        public string classMember { get; set; }
        public List<string> pendingInvitations { get; set; }
    }
}