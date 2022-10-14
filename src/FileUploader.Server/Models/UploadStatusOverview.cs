namespace FileUploader.Server.Models
{
    public class UploadStatusOverview
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public int FileLength { get; set; }
        public int ChunkLength { get; set; }
        public UploadStatus Status { get; set; }
        public ChunkStatus[] Chunks { get; set; }
    }
}
