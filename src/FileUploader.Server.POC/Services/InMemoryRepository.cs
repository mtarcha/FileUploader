using FileUploader.Server.POC.Contracts;
using FileUploader.Server.POC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Services
{
    internal class InMemoryRepository : IUploadsRepository
    {
        private readonly Dictionary<string, UploadStatusOverview> _inMemoryStorage = new();

        public Task<UploadStatusOverview> CreateAsync(UploadStatusOverview upload, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid().ToString();
            upload.Id = id;

            _inMemoryStorage.Add(id, upload);

            return Task.FromResult(upload);
        }

        public Task<UploadStatusOverview> GetAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_inMemoryStorage[id]);
        }

        public Task<UploadStatusOverview> UpdateStatusAsync(string id, UploadStatus newStatus, CancellationToken cancellationToken)
        {
            var upload = _inMemoryStorage[id];
            upload.Status = newStatus;

            return Task.FromResult(upload);
        }

        public Task<UploadStatusOverview> UpsertChunkStatusAsync(string id, ChunkStatus chunkStatus, CancellationToken cancellationToken)
        {
            var upload = _inMemoryStorage[id];
            var existingChunkStatus = upload.Chunks.FirstOrDefault(x => x.OrderNumber == chunkStatus.OrderNumber);

            if(existingChunkStatus != null)
            {
                upload.Chunks.Remove(existingChunkStatus);
            }

            upload.Chunks.Add(chunkStatus);
            upload.Status = UploadStatus.InProgress;

            return Task.FromResult(upload);
        }
    }
}
