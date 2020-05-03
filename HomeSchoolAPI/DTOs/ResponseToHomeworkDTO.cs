using System;

namespace HomeSchoolAPI.DTOs
{
    public class ResponseToHomeworkDTO
    {
        public string description { get; set; }
        public DateTime sendingTime { get; set; }
        public string homeworkID { get; set; }
        public string classID { get; set; }
    }
}