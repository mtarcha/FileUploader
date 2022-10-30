using FileUploader.Server.POC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileUploader.FunctionalTests
{
    public class FunctionalTests : IClassFixture<UploaderWebAppFactory>
    {
        private readonly HttpClient _uploaderApiClient;
        public FunctionalTests(UploaderWebAppFactory appFactory)
        {
            _uploaderApiClient = appFactory.CreateClient();
        }

        [Fact(DisplayName = "Uploaded file is ideantical to original file")]
        public async Task Test_01()
        {
            var fileNameToUpload = "Resources\\SmallFileToUpload.txt";
            var targetFileName = "bla.txt";
            var fileLengh = new FileInfo(fileNameToUpload).Length;
            var chunkSize = 100;

            // todo: throw if filename is path, or support it
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = targetFileName,
                FileLenght = fileLengh,
                ChunkLenght = chunkSize
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            ulong orderNumber = 1;
            foreach(var chunk in ReadFyleByChunks(fileNameToUpload, chunkSize))
            {
                var uploadChunk1Request = new UploadChunkRequest
                {
                    OrderNumber = orderNumber,
                    Data = chunk
                };

                await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk1Request);
                orderNumber++;
            }

            var statusResponse = await _uploaderApiClient.PatchAsync($"api/v1/uploads/{initStatus.Id}/complete", null);

            Assert.True(statusResponse.IsSuccessStatusCode);

            // todo: make target path configurable
            var areFilesEqual = AreFileContentsEqual(fileNameToUpload, $"D:\\UploaderTesting\\{initStatus.Id}\\result\\{targetFileName}");

            Assert.True(areFilesEqual);
        }

        private static IEnumerable<byte[]> ReadFyleByChunks(string fileName, int chunkSize)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var remainBytes = fs.Length;
                var bufferBytes = chunkSize;
                var chunk = new byte[chunkSize];
                while (remainBytes > 0)
                {
                    if (remainBytes < chunkSize)
                    {
                        chunk = new byte[remainBytes];
                        bufferBytes = (int)remainBytes;
                    }

                    fs.Read(chunk, 0, bufferBytes);

                    remainBytes -= bufferBytes;
                    yield return chunk;
                }
            }
        }

        private static bool AreFileContentsEqual(string path1, string path2)
        {
            using var md5 = MD5.Create();
            
            byte[] hash1 = null;
            byte[] hash2 = null;
            
            using (var stream = File.OpenRead(path1))
            {
                hash1 = md5.ComputeHash(stream);
            }

            using (var stream = File.OpenRead(path2))
            {
                hash2 = md5.ComputeHash(stream);
            }

            return hash1.SequenceEqual(hash2);
        }
    }
}
