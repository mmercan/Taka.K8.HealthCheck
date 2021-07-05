using System;
using System.Threading.Tasks;
using AutoMapper;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;

namespace Taka.K8s
{
    public class NamespaceWatchClient
    {
        private Kubernetes _client;
        private ILogger<NamespaceClient> _logger;
        private IMapper _mapper;

        public event EventHandler WatchTriggered;

        public NamespaceWatchClient(Kubernetes client, ILogger<NamespaceClient> logger, IMapper mapper)
        {
            this._client = client;
            this._logger = logger;
            this._mapper = mapper;
        }

        public async Task StartAsync()
        {
            // var podlistResp = _client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);
            var namespaceWatch = await _client.ListNamespaceWithHttpMessagesAsync(watch: true);
            using (namespaceWatch.Watch<V1Namespace, V1NamespaceList>((type, item) =>
            {

                _logger.LogCritical("==on watch event==");
                _logger.LogCritical(type.ToString());
                _logger.LogCritical(item.Metadata.Name);
                _logger.LogCritical("==on watch event==");
                OnWatchTriggered(new EventArgs());
            }))
            {
                _logger.LogCritical("Watch stopped");

            }

        }


        protected virtual void OnWatchTriggered(EventArgs e)
        {
            EventHandler handler = WatchTriggered;
            handler?.Invoke(this, e);
        }

    }
}