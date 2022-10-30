namespace FileUploader.Server.POC.Models
{
    public class ChunkStatus
    {
        public ulong OrderNumber { get; set; }
        public UploadStatus Status { get; set; }
    }
}
