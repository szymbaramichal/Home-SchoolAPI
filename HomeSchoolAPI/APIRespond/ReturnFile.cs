namespace HomeSchoolAPI.APIRespond
{
    public class ReturnFile
    {
        public System.IO.MemoryStream stream { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
    }
}