using FileUploader.Server.POC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Contracts
{
    public interface IUploadsRepository
    {
        Task<UploadStatusOverview> CreateAsync(UploadStatusOverview upload, CancellationToken cancellationToken);
        Task<UploadStatusOverview> UpdateStatusAsync(string id, UploadStatus newStatus, CancellationToken cancellationToken);
        Task<UploadStatusOverview> UpsertChunkStatusAsync(string id, ChunkStatus chunkStatus, CancellationToken cancellationToken);
        Task<UploadStatusOverview> GetAsync(string id, CancellationToken cancellationToken);
    }
}
