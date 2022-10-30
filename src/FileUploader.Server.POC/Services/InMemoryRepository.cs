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

        public Task<UploadStatusOverview> UpdateAsync(UploadStatusOverview upload, CancellationToken cancellationToken)
        {
            _inMemoryStorage[upload.Id] = upload;
            return Task.FromResult(upload);
        }
    }
}
