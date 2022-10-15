namespace FileUploader.Server.Models
{
    public class UploadChunkRequest
    {
        public ulong OrderNumber { get; set; }
        //public string Hash future
        public byte[] Data { get; set; }
    }
}
