using System.Collections.Generic;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.APIRespond
{
    public class UserToReturn
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string surrname { get; set; }
        public string email { get; set; }
        public int userRole { get; set; }
        public List<string> friends { get; set; }
        public string username { get; set; }
        public string userCode { get; set; }
    }
}