using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HomeSchoolAPI.DTOs
{
    public class HomeworkToAddDTO
    {
        public string classID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string time { get; set; }
    }
}