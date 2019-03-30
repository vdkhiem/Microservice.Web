using Microservice.WebApi.Interfaces;
using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.WebApi.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        IAdvertStorageService service;
        public StorageHealthCheck(IAdvertStorageService service)
        {
            this.service = service;
        }

        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var isServiceOK = await service.CheckHealthAsync();
            return HealthCheckResult.FromStatus(isServiceOK ? CheckStatus.Healthy : CheckStatus.Unhealthy, "");
        }

       
    }
}
