namespace HomeSchoolCore.APIRequest
{
    public class PutMarkDTO
    {
        public string ResponseID { get; set; }
        public string HomeworkID { get; set; }
        public string SubjectID { get; set; }
        public string ClassID { get; set; }
        public string Mark { get; set; }
    }
}