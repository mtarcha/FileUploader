using FileUploader.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Server.Controllers
{
    [ApiController]
    [Route("api/v1/uploads")]
    public class UploadsController : ControllerBase
    {
       
        private readonly ILogger<UploadsController> _logger;

        public UploadsController(ILogger<UploadsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("init")]
        public UploadStatusOverview Initiate(InitiateFileUploadRequest reqest)
        {
            return new UploadStatusOverview();
        }

        [HttpGet("{id}")]
        public UploadStatusOverview Get(string id)
        {
            return new UploadStatusOverview();
        }

        [HttpPost("{id}/chunks")]
        public ChunkStatus UploadChunk(int id, UploadChunkRequest reqest)
        {
            return new ChunkStatus();
        }

        [HttpPatch("{id}/complete")]
        public UploadStatusOverview Complete(string id)
        {
            return new UploadStatusOverview();
        }

        [HttpPatch("{id}/abort")]
        public UploadStatusOverview Abort(string id)
        {
            return new UploadStatusOverview();
        }
    }
}