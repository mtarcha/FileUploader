using FileUploader.Server.POC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileUploader.FunctionalTests
{
    public class UploaderApiTests : IClassFixture<UploaderWebAppFactory>
    {
        private readonly HttpClient _uploaderApiClient;
        public UploaderApiTests(UploaderWebAppFactory appFactory)
        {
            _uploaderApiClient = appFactory.CreateClient();
        }

        [Fact(DisplayName = "Can initiate new Upload")]
        public async Task Test_01()
        {
            var request = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 10,
                ChunkLenght = 1
            };

            var initiateResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", request);

            Assert.True(initiateResponse.IsSuccessStatusCode);

            var responseStatus = await initiateResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();
            Assert.NotNull(responseStatus);
            Assert.NotNull(responseStatus.Id);
            Assert.Equal(request.FileName, responseStatus.FileName);
            Assert.Equal(request.FileLenght, responseStatus.FileLength);
            Assert.Equal(request.ChunkLenght, responseStatus.ChunkLength);
            Assert.Empty(responseStatus.Chunks);
            Assert.Equal(UploadStatus.Initiated, responseStatus.Status);
        }

        [Fact(DisplayName = "Can upload chunk of file successfully")]
        public async Task Test_02()
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 10,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var uploadChunkRequest = new UploadChunkRequest
            {
                OrderNumber = 1,
                Data = new byte[5] { 0x01, 0x01, 0x02, 0x01, 0x01 }
            };

            var uploadResponse = await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunkRequest);

            Assert.True(uploadResponse.IsSuccessStatusCode);

            var chunkStatus = await uploadResponse.Content.ReadFromJsonAsync<ChunkStatus>();
            Assert.NotNull(chunkStatus);
            Assert.Equal(uploadChunkRequest.OrderNumber, chunkStatus.OrderNumber);
            Assert.Equal(UploadStatus.Uploaded, chunkStatus.Status);


            var statusResponse = await _uploaderApiClient.GetAsync($"api/v1/uploads/{initStatus.Id}");

            Assert.True(statusResponse.IsSuccessStatusCode);

            var uplodStatus = await statusResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();
            Assert.NotNull(uplodStatus);
            Assert.Equal(initStatus.Id, uplodStatus.Id);
            Assert.Equal(initRequest.FileName, uplodStatus.FileName);
            Assert.Equal(initRequest.FileLenght, uplodStatus.FileLength);
            Assert.Equal(initRequest.ChunkLenght, uplodStatus.ChunkLength);
            Assert.Single(uplodStatus.Chunks);
            Assert.Equal(UploadStatus.InProgress, uplodStatus.Status);
        }

        [Fact(DisplayName = "Last uploaded chunk can have smaller lenght")]
        public async Task Test_03()
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 9,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var uploadChunk1Request = new UploadChunkRequest
            {
                OrderNumber = 1,
                Data = new byte[5] { 0x01, 0x01, 0x02, 0x01, 0x01 }
            };

            await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk1Request);

            var uploadChunk2Request = new UploadChunkRequest
            {
                OrderNumber = 2,
                Data = new byte[4] { 0x01, 0x01, 0x02, 0x01 }
            };

            var lastChunkUploadResponse = await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk2Request);

            Assert.True(lastChunkUploadResponse.IsSuccessStatusCode);

            var statusResponse = await _uploaderApiClient.GetAsync($"api/v1/uploads/{initStatus.Id}");

            Assert.True(statusResponse.IsSuccessStatusCode);

            var uplodStatus = await statusResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();
            Assert.NotNull(uplodStatus);
            Assert.Equal(initStatus.Id, uplodStatus.Id);
            Assert.Equal(initRequest.FileName, uplodStatus.FileName);
            Assert.Equal(initRequest.FileLenght, uplodStatus.FileLength);
            Assert.Equal(initRequest.ChunkLenght, uplodStatus.ChunkLength);
            Assert.Equal(2, uplodStatus.Chunks.Count);
            Assert.Equal(UploadStatus.InProgress, uplodStatus.Status);
        }

        [Theory(DisplayName = "Non last chunk cannot have wrong size")]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public async Task Test_04(int chunkSize)
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 10,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var rnd = new Random();
            var chunkData = new byte[chunkSize];
            rnd.NextBytes(chunkData);

            var uploadChunkRequest = new UploadChunkRequest
            {
                OrderNumber = 1,
                Data = chunkData
            };

            var uploadResponse = await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunkRequest);

            Assert.Equal(HttpStatusCode.BadRequest, uploadResponse.StatusCode);
        }

        [Fact(DisplayName = "Can rewrite chunk of file successfully")]
        public async Task Test_05()
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 10,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var rnd = new Random();
            var chunkData = new byte[5];

            for(int i = 0; i < 7; i++)
            {
                rnd.NextBytes(chunkData);

                var uploadChunkRequest = new UploadChunkRequest
                {
                    OrderNumber = 1,
                    Data = chunkData
                };

                var uploadResponse = await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunkRequest);

                Assert.True(uploadResponse.IsSuccessStatusCode);
            }

            var statusResponse = await _uploaderApiClient.GetAsync($"api/v1/uploads/{initStatus.Id}");

            Assert.True(statusResponse.IsSuccessStatusCode);

            var uplodStatus = await statusResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();
            Assert.NotNull(uplodStatus);
            Assert.Equal(initStatus.Id, uplodStatus.Id);
            Assert.Equal(initRequest.FileName, uplodStatus.FileName);
            Assert.Equal(initRequest.FileLenght, uplodStatus.FileLength);
            Assert.Equal(initRequest.ChunkLenght, uplodStatus.ChunkLength);
            Assert.Single(uplodStatus.Chunks);
            Assert.Equal(UploadStatus.InProgress, uplodStatus.Status);
        }

        [Fact(DisplayName = "Can complete upload when all chunks uploaded")]
        public async Task Test_06()
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 9,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var uploadChunk1Request = new UploadChunkRequest
            {
                OrderNumber = 1,
                Data = new byte[5] { 0x01, 0x01, 0x02, 0x01, 0x01 }
            };

            await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk1Request);

            var uploadChunk2Request = new UploadChunkRequest
            {
                OrderNumber = 2,
                Data = new byte[4] { 0x01, 0x01, 0x02, 0x01 }
            };

            await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk2Request);

            var statusResponse = await _uploaderApiClient.PatchAsync($"api/v1/uploads/{initStatus.Id}/complete", null);

            Assert.True(statusResponse.IsSuccessStatusCode);
        }

        [Fact(DisplayName = "Cannot complete upload when not all chunks uploaded")]
        public async Task Test_07()
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 9,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var uploadChunk1Request = new UploadChunkRequest
            {
                OrderNumber = 1,
                Data = new byte[5] { 0x01, 0x01, 0x02, 0x01, 0x01 }
            };

            await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk1Request);

            var statusResponse = await _uploaderApiClient.PatchAsync($"api/v1/uploads/{initStatus.Id}/complete", null);

            Assert.Equal(HttpStatusCode.BadRequest, statusResponse.StatusCode);
        }

        [Fact(DisplayName = "Can abort any time")]
        public async Task Test_08()
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 9,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var uploadChunk1Request = new UploadChunkRequest
            {
                OrderNumber = 1,
                Data = new byte[5] { 0x01, 0x01, 0x02, 0x01, 0x01 }
            };

            await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk1Request);

            var statusResponse = await _uploaderApiClient.PatchAsync($"api/v1/uploads/{initStatus.Id}/abort", null);

            Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);

            var uplodStatus = await statusResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();
            Assert.NotNull(uplodStatus);
            Assert.Equal(initStatus.Id, uplodStatus.Id);
            Assert.Equal(initRequest.FileName, uplodStatus.FileName);
            Assert.Equal(initRequest.FileLenght, uplodStatus.FileLength);
            Assert.Equal(initRequest.ChunkLenght, uplodStatus.ChunkLength);
            Assert.Equal(UploadStatus.Aborted, uplodStatus.Status);
        }

        [Fact(DisplayName = "After abort cannot upload anymore")]
        public async Task Test_09()
        {
            var initRequest = new InitiateFileUploadRequest
            {
                FileName = "bla.txt",
                FileLenght = 9,
                ChunkLenght = 5
            };

            var initResponse = await _uploaderApiClient.PostAsJsonAsync("api/v1/uploads/init", initRequest);
            var initStatus = await initResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();

            var uploadChunk1Request = new UploadChunkRequest
            {
                OrderNumber = 1,
                Data = new byte[5] { 0x01, 0x01, 0x02, 0x01, 0x01 }
            };

            await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk1Request);

            var statusResponse = await _uploaderApiClient.PatchAsync($"api/v1/uploads/{initStatus.Id}/abort", null);

            Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);

            var uploadChunk2Request = new UploadChunkRequest
            {
                OrderNumber = 2,
                Data = new byte[4] { 0x01, 0x01, 0x02, 0x01 }
            };

            var lastChunkUploadResponse = await _uploaderApiClient.PostAsJsonAsync($"api/v1/uploads/{initStatus.Id}/chunks", uploadChunk2Request);

            Assert.Equal(HttpStatusCode.BadRequest, lastChunkUploadResponse.StatusCode);

            var uplodStatus = await statusResponse.Content.ReadFromJsonAsync<UploadStatusOverview>();
            Assert.NotNull(uplodStatus);
            Assert.Equal(initStatus.Id, uplodStatus.Id);
            Assert.Equal(initRequest.FileName, uplodStatus.FileName);
            Assert.Equal(initRequest.FileLenght, uplodStatus.FileLength);
            Assert.Equal(initRequest.ChunkLenght, uplodStatus.ChunkLength);
            Assert.Equal(UploadStatus.Aborted, uplodStatus.Status);
        }
    }
}
