using System.Collections.Generic;
using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class ClassToReturn
    {
        public string Id { get; set; }
        public string CreatorID { get; set; }
        public string ClassName { get; set; }
        public string SchoolName { get; set; }
        public int MembersAmount { get; set; }
        public List<string> Members { get; set; }
        public List<SubjectToReturn> Subjects { get; set; }
    }
}