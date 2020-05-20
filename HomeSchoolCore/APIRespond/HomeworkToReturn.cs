using System;
using System.Collections.Generic;
using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class HomeworkToReturn
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string subjectID { get; set; }
        public List<Response> responses { get; set; }
        public DateTime createDate { get; set; }
        public DateTime endDate { get; set; }
        public string[] files { get; set; }
        public string[] linkHrefs { get; set; }
    }
}