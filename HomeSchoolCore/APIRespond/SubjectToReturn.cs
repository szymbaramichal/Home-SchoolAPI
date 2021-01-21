using System.Collections.Generic;
using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class SubjectToReturn
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ClassID { get; set; }
        public string TeacherID { get; set; }
        public List<HomeworkToReturn> Homeworks { get; set; }
        public QuizesToReturn Quizes { get; set; } = new QuizesToReturn {
            Answers = new List<AnswerToReturn>(),
            Quizes = new List<Quiz>() 
        };
    }
}