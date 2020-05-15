namespace FileStorageAPI.ApiResponse
{
    public class FileObject
    {
        public System.IO.MemoryStream stream { get; set; }
        public string contentType { get; set; }
    }
}