using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.Core.JobProcessing
{
    internal interface ILongJob
    {
        string Id { get; set; }
        JobStatus Status { get; set; }
        IDictionary<string, string> Context { get; set; }

        Task PerformAsync(CancellationToken cancellationToken);
    }
}
