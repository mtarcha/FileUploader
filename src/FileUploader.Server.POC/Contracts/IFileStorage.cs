using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Contracts
{
    public interface IFileStorage
    {
        Task InitiateUploadAsync(string id, CancellationToken cancellationToken);
        Task UploadChunkAsync(string id, ulong orderNumber, byte[] chunk, CancellationToken cancellationToken);
        Task MergeChunksAsync(string id, string FileName, CancellationToken cancellationToken);
        Task<string?> GetFileLocationAsync(string id, CancellationToken cancellationToken);
        Task DeleteChunksAsync(string id, CancellationToken cancellationToken);
    }
}
