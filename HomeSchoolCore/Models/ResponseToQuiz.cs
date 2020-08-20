using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSchoolCore.Models
{
    public class ResponseToQuiz
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string quizId { get; set; }
        public string classId { get; set; }
        public string executonerId { get; set; }
        public int correctAnswers { get; set; }
        public double percentageOfCorrectAnswers { get; set; }
        public DateTime FinishDate { get; set; }
    }
}