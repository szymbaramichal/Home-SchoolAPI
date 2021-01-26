using System.Collections.Generic;

namespace HomeSchoolCore.APIRespond
{
    public class QuestionToReturn
    {
        public string question { get; set; }
        public List<string> Answers { get; set; } = new List<string>();
    }
}