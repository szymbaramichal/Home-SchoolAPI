using System;
using System.Collections.Generic;

namespace HomeSchoolCore.APIRequest
{
    public class HomeworkToAddDTO
    {
        public string SubjectID { get; set; }
        public string ClassID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public List<string> FilesID { get; set; }
        public List<string> LinkHrefs { get; set; }
    }
}