using FileUploader.Server.POC.Contracts;
using FileUploader.Server.POC.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploader.Server.POC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileUploader(this IServiceCollection services)
        {
            services.AddScoped<IFileUploaderService, FileUploaderService>();
            services.AddScoped<IFileStorage, LocalFileStorage>();
            services.AddSingleton<IUploadsRepository, InMemoryRepository>();

            return services;
        }
    }
}
