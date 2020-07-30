using System.Collections.Generic;
using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class UserToReturn
    {        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surrname { get; set; }
        public string Email { get; set; }
        public int UserRole { get; set; } //0 -> student
        public string UserCode { get; set; } // ID of class
        public List<string> PendingInvitations { get; set; }
    }
}