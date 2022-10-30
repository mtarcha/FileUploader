namespace FileUploader.Server.POC.Models
{
    public class UploadChunkRequest
    {
        public ulong OrderNumber { get; set; }
        //public string Hash future
        public byte[] Data { get; set; }
    }
}
