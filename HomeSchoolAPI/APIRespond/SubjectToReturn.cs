using System.Collections.Generic;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.APIRespond
{
    public class SubjectToReturn
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string classID { get; set; }
        public string teacherID { get; set; }
        public List<Homework> homeworks { get; set; }
    }
}