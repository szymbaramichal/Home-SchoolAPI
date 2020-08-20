using System;
using System.Collections.Generic;
using HomeSchoolCore.Helpers;

namespace HomeSchoolCore.APIRequest
{
    public class CreateQuizDTO
    {
        public string name { get; set; }
        public string classID { get; set; }
        public string subjectID { get; set; }
        public List<Question> questions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
    }
}