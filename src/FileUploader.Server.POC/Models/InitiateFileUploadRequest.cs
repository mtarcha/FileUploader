namespace FileUploader.Server.POC.Models
{
    public class InitiateFileUploadRequest
    {
        public string FileName { get; set; }
        //public string FileHash { get; set; } future
        public long FileLenght { get; set; }
        public int ChunkLenght { get; set; }
    }
}
