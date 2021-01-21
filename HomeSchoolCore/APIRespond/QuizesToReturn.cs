using System.Collections.Generic;
using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class QuizesToReturn
    {
        public List<Quiz> Quizes { get; set; }
        public List<AnswerToReturn> Answers { get; set; }
    }
}