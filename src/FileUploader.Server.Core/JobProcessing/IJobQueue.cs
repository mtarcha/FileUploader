using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.Core.JobProcessing
{
    public interface IJobQueue<TJob>
    {
        Task<TJob> DequeueAsync(CancellationToken token);

        Task EnqueueAsync(TJob job, CancellationToken token);
    }
}
