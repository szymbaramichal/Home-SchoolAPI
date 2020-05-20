using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class ResponseReturn
    {
        public Response responseObj { get; set; }
        public Homework homeworkObj { get; set; }
        public string homeworkName { get; set; }
    }
}