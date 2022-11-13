using FileUploader.Server.POC.Contracts;
using FileUploader.Server.POC.Models;

namespace FileUploader.Server.POC.Services
{
    // todo: consider using sagas to manage upload process
    internal class FileUploaderService : IFileUploaderService
    {
        private readonly IUploadsRepository _statusRpository;
        private readonly IFileStorage _fileStorage;

        public FileUploaderService(IUploadsRepository statusRpository, IFileStorage fileStorage)
        {
            _statusRpository = statusRpository;
            _fileStorage = fileStorage;
        }

        public async Task<UploadStatusOverview> InitiateUploadAsync(InitiateFileUploadRequest reqest, CancellationToken cancellationToken)
        {
            var status = await _statusRpository.CreateAsync(new UploadStatusOverview
            {
                FileName = reqest.FileName,
                FileLength = reqest.FileLenght,
                ChunkLength = reqest.ChunkLenght,
                Status = UploadStatus.Initiated

            }, cancellationToken);

            await _fileStorage.InitiateUploadAsync(status.Id, cancellationToken);

            return status;
        }

        public async Task<UploadStatusOverview> GetStatusAsync(string id, CancellationToken cancellationToken)
        {
            return await _statusRpository.GetAsync(id, cancellationToken);
        }

        public async Task<ChunkStatus> UploadChunkAsync(string id, UploadChunkRequest reqest, CancellationToken cancellationToken)
        {
            var status = await _statusRpository.GetAsync(id, cancellationToken);
            //todo: validate

            await _fileStorage.UploadChunkAsync(id, reqest.OrderNumber, reqest.Data, cancellationToken);

            var chunkStatus = new ChunkStatus
            {
                OrderNumber = reqest.OrderNumber,
                Status = UploadStatus.Uploaded
            };

            // todo: this should not fail upload 
            await _statusRpository.UpsertChunkStatusAsync(id, chunkStatus, cancellationToken);

            return chunkStatus;
        }

        public async Task<UploadStatusOverview> AbortUploadAsync(string id, CancellationToken cancellationToken)
        {
            await _fileStorage.DeleteChunksAsync(id, cancellationToken);

            // todo: this should not fail abort 
            return await _statusRpository.UpdateStatusAsync(id, UploadStatus.Aborted, cancellationToken);
           
        }

        public async Task<UploadStatusOverview> CompleteUploadAsync(string id, CancellationToken cancellationToken)
        {
            var status = await _statusRpository.GetAsync(id, cancellationToken);
            // todo: validate

            await _fileStorage.StartMergeChunksAsync(id, status.FileName, cancellationToken);
            var location = await _fileStorage.GetFileLocationAsync(id, cancellationToken);

            //todo set target location

            // todo: this should not fail upload 
            await _statusRpository.UpdateStatusAsync(id, UploadStatus.Uploaded, cancellationToken);

            return status;
        }
    }
}
