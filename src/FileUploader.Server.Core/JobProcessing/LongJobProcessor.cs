using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.Core.JobProcessing
{
    internal class LongJobProcessor : BackgroundService
    {
        private readonly IJobQueue<ILongJob> _jobQueue;

        public LongJobProcessor(IJobQueue<ILongJob> jobQueue)
        {
            _jobQueue = jobQueue;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested)
            {
                var job = await _jobQueue.DequeueAsync(stoppingToken);

                await job.PerformAsync(stoppingToken);

                await _jobQueue.UpdateAsync(job, stoppingToken);
            }
        }
    }
}
