using System;
using System.Threading.Tasks;
using AutoMapper;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Taka.Models;

namespace Taka.K8s
{

    public class TakaNamespaceEventArgs : EventArgs
    {
        public TakaNamespaceEventArgs(TakaNamespace takaNamespace, WatchEventType eventType)
        {

        }
    }


    public class NamespaceWatchClient
    {
        private readonly Kubernetes _client;
        private readonly ILogger<NamespaceClient> _logger;
        private readonly IMapper _mapper;

        //  public delegate void TakaNamespaceEventHandler(object sender, TakaNamespaceEventArgs args);
        // public event TakaNamespaceEventHandler WatchTriggered;

        public event EventHandler<TakaNamespaceEventArgs> WatchTriggered;
        public event EventHandler<TakaNamespaceEventArgs> TakaNamespaceAdded;
        public event EventHandler<TakaNamespaceEventArgs> TakaNamespaceRemoved;
        public event EventHandler<TakaNamespaceEventArgs> TakaNamespaceUpdated;

        public NamespaceWatchClient(Kubernetes client, ILogger<NamespaceClient> logger, IMapper mapper)
        {
            this._client = client;
            this._logger = logger;
            this._mapper = mapper;
        }

        public async Task StartAsync()
        {
            var namespaceWatch = await _client.ListNamespaceWithHttpMessagesAsync(watch: true);
            using (namespaceWatch.Watch<V1Namespace, V1NamespaceList>((type, item) =>
            {

                _logger.LogCritical("==on watch event==");
                _logger.LogCritical(type.ToString());
                _logger.LogCritical(item.Metadata.Name);
                _logger.LogCritical("==on watch event==");
                var mappeditem = _mapper.Map<TakaNamespace>(item);
                OnWatchTriggered(new TakaNamespaceEventArgs(mappeditem, type));
                switch (type)
                {

                    case WatchEventType.Added:
                        {
                            OnTakaNamespaceAdded(new TakaNamespaceEventArgs(mappeditem, type));
                            break;
                        }
                    case WatchEventType.Deleted:
                        {
                            OnTakaNamespaceRemoved(new TakaNamespaceEventArgs(mappeditem, type));
                            break;
                        }
                    case WatchEventType.Modified:
                        {
                            OnTakaNamespaceUpdated(new TakaNamespaceEventArgs(mappeditem, type));
                            break;
                        }

                    default: break;
                }
            }))
            {
                _logger.LogCritical("Watch stopped");
            }
        }


        protected virtual void OnWatchTriggered(TakaNamespaceEventArgs e)
        {
            EventHandler<TakaNamespaceEventArgs> handler = WatchTriggered;
            handler?.Invoke(this, e);
        }

        protected virtual void OnTakaNamespaceAdded(TakaNamespaceEventArgs e)
        {
            EventHandler<TakaNamespaceEventArgs> handler = TakaNamespaceAdded;
            handler?.Invoke(this, e);
        }

        protected virtual void OnTakaNamespaceRemoved(TakaNamespaceEventArgs e)
        {
            EventHandler<TakaNamespaceEventArgs> handler = TakaNamespaceRemoved;
            handler?.Invoke(this, e);
        }

        protected virtual void OnTakaNamespaceUpdated(TakaNamespaceEventArgs e)
        {
            EventHandler<TakaNamespaceEventArgs> handler = TakaNamespaceUpdated;
            handler?.Invoke(this, e);
        }
    }
}