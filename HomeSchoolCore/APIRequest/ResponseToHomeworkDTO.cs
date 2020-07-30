using System;
using System.Collections.Generic;

namespace HomeSchoolCore.APIRequest
{
    public class ResponseToHomeworkDTO
    {
        public string Description { get; set; }
        public DateTime SendingTime { get; set; }
        public string HomeworkID { get; set; }
        public string ClassID { get; set; }
        public List<string> FilesID { get; set; }
        public List<string> LinkHrefs { get; set; }
    }
}