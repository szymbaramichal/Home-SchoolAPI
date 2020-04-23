using System.Collections.Generic;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.APIRespond
{
    public class ClassToReturn
    {
        public string Id { get; set; }
        public string creatorID { get; set; }
        public string className { get; set; }
        public string schoolName { get; set; }
        public int membersAmount { get; set; }
        public List<string> members { get; set; }
        public List<Subject> subjects { get; set; }
    }
}