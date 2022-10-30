using FileUploader.Server.POC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Contracts
{
    public interface IFileUploaderService
    {
        Task<UploadStatusOverview> InitiateUploadAsync(InitiateFileUploadRequest reqest, CancellationToken cancellationToken);

        Task<UploadStatusOverview> GetStatusAsync(string id, CancellationToken cancellationToken);

        Task<ChunkStatus> UploadChunkAsync(string id, UploadChunkRequest reqest, CancellationToken cancellationToken);

        Task<UploadStatusOverview> CompleteUploadAsync(string id, CancellationToken cancellationToken);

        Task<UploadStatusOverview> AbortUploadAsync(string id, CancellationToken cancellationToken);
    }
}
