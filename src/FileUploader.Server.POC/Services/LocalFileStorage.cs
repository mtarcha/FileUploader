using FileUploader.Server.POC.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Services
{
    internal class LocalFileStorage : IFileStorage
    {
        private readonly string _location;

        public LocalFileStorage()
        {
            _location = "D:\\UploaderTesting";
        }

        public Task InitiateUploadAsync(string id, CancellationToken cancellationToken)
        {
            var path = Path.Combine(_location, id);
            Directory.CreateDirectory(path);

            return Task.CompletedTask;
        }

        public Task DeleteChunksAsync(string id, CancellationToken cancellationToken)
        {
            var path = Path.Combine(_location, id);
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public Task<string?> GetFileLocationAsync(string id, CancellationToken cancellationToken)
        {
            var path = Path.Combine(_location, id, "result");

            var fileFullName = Directory.GetFiles(path).FirstOrDefault();

            return Task.FromResult(fileFullName);
        }

        public async Task MergeChunksAsync(string id, string FileName, CancellationToken cancellationToken)
        {
            var inputChunksPath = Path.Combine(_location, id);
            var outputDirectory = Path.Combine(_location, id, "result");
            var outputFilePath = Path.Combine(outputDirectory, FileName);

            var inputFilePaths = Directory.GetFiles(inputChunksPath, "*.bin", new EnumerationOptions
            {
                RecurseSubdirectories = false
            }).OrderBy(x => x);

            Directory.CreateDirectory(outputDirectory);
            //using var file = File.Create(outputFilePath);
            
            using (var outputStream = File.Open(outputFilePath, FileMode.Append))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        await inputStream.CopyToAsync(outputStream, cancellationToken);
                    }

                    Console.WriteLine("The file {0} has been processed.", inputFilePath);
                }
            }
        }

        public Task UploadChunkAsync(string id, ulong orderNumber, byte[] chunk, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_location, id, $"{orderNumber}.bin");
            using var writer = new BinaryWriter(File.OpenWrite(filePath));
            writer.Write(chunk);

            return Task.CompletedTask;
        }
    }
}
