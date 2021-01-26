using System;
using System.Collections.Generic;

namespace HomeSchoolCore.APIRespond
{
    public class QuizToReturn
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string classID { get; set; }
        public string subjectID { get; set; }
        public int amountOfQuestions { get; set; }
        public List<QuestionToReturn> questions { get; set; }
        public List<string> executonersId { get; set; } = new List<string>();
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        //ACTIVE, INACTIVE
        public string status { get; set; }
    }
}