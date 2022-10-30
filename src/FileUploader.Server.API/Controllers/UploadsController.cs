using FileUploader.Server.POC.Models;
using FileUploader.Server.POC.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Server.API.Controllers
{
    [ApiController]
    [Route("api/v1/uploads")]
    public class UploadsController : ControllerBase
    {
        private readonly IFileUploaderService _uploaderService;

        public UploadsController(IFileUploaderService uploaderService)
        {
            _uploaderService = uploaderService;
        }

        [HttpPost("init")]
        public async Task<UploadStatusOverview> Initiate(InitiateFileUploadRequest reqest, CancellationToken cancellationToken)
        {
            return await _uploaderService.InitiateUploadAsync(reqest, cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<UploadStatusOverview> Get(string id, CancellationToken cancellationToken)
        {
            return await _uploaderService.GetStatusAsync(id, cancellationToken);
        }

        [HttpPost("{id}/chunks")]
        public async Task<ChunkStatus> UploadChunk(string id, UploadChunkRequest reqest, CancellationToken cancellationToken)
        {
            return await _uploaderService.UploadChunkAsync(id, reqest, cancellationToken);
        }

        [HttpPatch("{id}/complete")]
        public async Task<UploadStatusOverview> Complete(string id, CancellationToken cancellationToken)
        {
            return await _uploaderService.CompleteUploadAsync(id, cancellationToken);
        }

        [HttpPatch("{id}/abort")]
        public async Task<UploadStatusOverview> Abort(string id, CancellationToken cancellationToken)
        {
            return await _uploaderService.AbortUploadAsync(id, cancellationToken);
        }
    }
}