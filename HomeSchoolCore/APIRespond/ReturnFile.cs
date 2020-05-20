namespace HomeSchoolCore.APIRespond
{
    public class ReturnFile
    {
        public System.IO.MemoryStream stream { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public string senderID { get; set; }
        public string subjectID { get; set; }
        public string classID { get; set; }
    }
}