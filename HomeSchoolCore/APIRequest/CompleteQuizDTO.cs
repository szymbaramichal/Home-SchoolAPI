using System;
using System.Collections.Generic;

namespace HomeSchoolCore.APIRequest
{
    public class CompleteQuizDTO
    {
        public string quizId { get; set; }
        public string classId { get; set; }
        public List<string> answers { get; set; }
        public DateTime FinishDate { get; set; }
        
    }
}