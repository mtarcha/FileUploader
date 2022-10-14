namespace FileUploader.Server.Models
{
    public class UploadChunkRequest
    {
        public int OrderNumber { get; set; }
        //public string Hash future
        public byte[] Data { get; set; }
    }
}
