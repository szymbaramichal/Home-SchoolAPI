using HomeSchoolCore.Models;

namespace HomeSchoolCore.APIRespond
{
    public class ResponseReturn
    {
        public Response ResponseObj { get; set; }
        public Homework HomeworkObj { get; set; }
        public string HomeworkName { get; set; }
    }
}