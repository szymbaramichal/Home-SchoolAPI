using System;
using System.Collections.Generic;
using HomeSchoolCore.APIRequest;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolCore.Models
{
    public class Quiz
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public string classID { get; set; }
        public string subjectID { get; set; }
        public int amountOfQuestions { get; set; }
        public List<Question> questions { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        //ACTIVE, INACTIVE
        public string status { get; set; }
    }
}