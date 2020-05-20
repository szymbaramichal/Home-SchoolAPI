using System.Collections.Generic;
using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class SubjectToReturn
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string classID { get; set; }
        public string teacherID { get; set; }
        public List<HomeworkToReturn> homeworks { get; set; }
    }
}