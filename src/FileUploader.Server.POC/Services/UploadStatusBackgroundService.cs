using FileUploader.Server.POC.Contracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Services
{
    internal class UploadStatusBackgroundService : BackgroundService
    {
        private readonly IUploadsRepository _repository;
        private readonly IFileStorage _fileStorage;

        UploadStatusBackgroundService(IUploadsRepository repository, IFileStorage fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // todo:
            // pull in merge progress statuses and check file storage status
            // update staus in db if changed
            throw new NotImplementedException();
        }
    }
}
