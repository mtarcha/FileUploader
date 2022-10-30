namespace FileUploader.Server.POC.Models
{
    public class UploadStatusOverview
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string? TargetLocation { get; set; }
        public long FileLength { get; set; }
        public int ChunkLength { get; set; }
        public UploadStatus Status { get; set; }

        // temporary list
        public IList<ChunkStatus> Chunks { get; set; } = new List<ChunkStatus>();
    }
}
