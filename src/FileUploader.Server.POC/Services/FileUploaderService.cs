using FileUploader.Server.POC.Contracts;
using FileUploader.Server.POC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Services
{
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

            status.Chunks.Add(chunkStatus);
            status.Status = UploadStatus.InProgress;

            await _statusRpository.UpdateAsync(status, cancellationToken);

            return chunkStatus;
        }

        public async Task<UploadStatusOverview> AbortUploadAsync(string id, CancellationToken cancellationToken)
        {
            var status = await _statusRpository.GetAsync(id, cancellationToken);
            // todo: validate

            await _fileStorage.DeleteChunksAsync(id, cancellationToken);

            status.Status = UploadStatus.Aborted;

            await _statusRpository.UpdateAsync(status, cancellationToken);
            
            return status;
        }

        public async Task<UploadStatusOverview> CompleteUploadAsync(string id, CancellationToken cancellationToken)
        {
            var status = await _statusRpository.GetAsync(id, cancellationToken);
            // todo: validate

            await _fileStorage.MergeChunksAsync(id, status.FileName, cancellationToken);
            var location = await _fileStorage.GetFileLocationAsync(id, cancellationToken);

            status.Status = UploadStatus.Uploaded;
            status.TargetLocation = location;

            await _statusRpository.UpdateAsync(status, cancellationToken);

            return status;
        }
    }
}
