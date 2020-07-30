using System;
using System.Collections.Generic;
using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class HomeworkToReturn
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubjectID { get; set; }
        public List<Response> Responses { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EndDate { get; set; }
        public string[] Files { get; set; }
        public string[] LinkHrefs { get; set; }
    }
}