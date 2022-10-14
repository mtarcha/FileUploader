using FileUploader.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadsController : ControllerBase
    {
       
        private readonly ILogger<UploadsController> _logger;

        public UploadsController(ILogger<UploadsController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "init")]
        public UploadStatusOverview Initiate(InitiateFileUploadRequest reqest)
        {
            return new UploadStatusOverview();
        }

        [HttpGet(Name = "{id}")]
        public UploadStatusOverview Get(string id)
        {
            return new UploadStatusOverview();
        }

        [HttpPost(Name = "{id}/chunks")]
        public ChunkStatus UploadChunk(int id, UploadChunkRequest reqest)
        {
            return new ChunkStatus();
        }

        [HttpPatch(Name = "{id}/complete")]
        public UploadStatusOverview Complete(string id)
        {
            return new UploadStatusOverview();
        }

        [HttpPatch(Name = "{id}/abort")]
        public UploadStatusOverview Abort(string id)
        {
            return new UploadStatusOverview();
        }
    }
}