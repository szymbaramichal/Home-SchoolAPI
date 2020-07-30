namespace HomeSchoolCore.APIRespond
{
    public class ReturnFile
    {
        public System.IO.MemoryStream Stream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string SenderID { get; set; }
        public string SubjectID { get; set; }
        public string ClassID { get; set; }
    }
}