using System;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Taka.K8s;

namespace Taka.Worker.Sync.Services
{
    public class NamespaceWatcher : BackgroundService
    {
        private readonly global::k8s.Kubernetes _client;
        private readonly ILogger<NamespaceWatcher> _logger;

        public NamespaceWatcher(IKubernetesClient client, ILogger<NamespaceWatcher> logger)
        {
            _client = client.Client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(stoppingToken);
            }
        }

        private Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
             {
                 var nslistResp = _client.ListNamespaceWithHttpMessagesAsync(watch: true);
                 using (nslistResp.Watch<V1Namespace, V1NamespaceList>((type, item) =>
                 {
                     _logger.LogCritical(DateTime.Now + " ==on watch event==");
                     _logger.LogCritical(type.ToString());
                     _logger.LogCritical(item.Metadata.Name);
                     _logger.LogCritical(DateTime.Now + " ==on watch event==");
                 }))
                 {
                     _logger.LogCritical("press ctrl + c to stop watching");

                     var ctrlc = new ManualResetEventSlim(false);
                     Console.CancelKeyPress += (sender, eventArgs) => ctrlc.Set();
                     ctrlc.Wait();
                 }
             }, cancellationToken);
        }
    }
}