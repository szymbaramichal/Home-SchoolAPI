namespace HomeSchoolAPI.DTOs
{
    public class PutMarkDTO
    {
        public string responseID { get; set; }
        public string homeworkID { get; set; }
        public string subjectID { get; set; }
        public string classID { get; set; }
        public string mark { get; set; }
    }
}