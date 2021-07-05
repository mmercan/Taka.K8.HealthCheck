using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Taka.Models;

namespace Taka.K8s
{
    public class NamespaceClient
    {
        private Kubernetes _client;
        private ILogger<NamespaceClient> _logger;
        private IMapper _mapper;


        public NamespaceClient(Kubernetes client, ILogger<NamespaceClient> logger, IMapper mapper)
        {
            this._client = client;
            this._logger = logger;
            this._mapper = mapper;
        }

        public IList<V1Namespace> GetAll()
        {
            var namespaces = _client.ListNamespace().Items;
            return namespaces;
        }

        public async Task<IList<V1Namespace>> GetAllAsync()
        {
            var namespaces = await _client.ListNamespaceAsync();
            return namespaces.Items;
        }

        public IList<TakaNamespace> MappedGetAll()
        {

            IList<V1Namespace> namespaces = GetAll();
            var mappedns = _mapper.Map<List<TakaNamespace>>(namespaces);
            return mappedns;
        }


        public async Task<IList<TakaNamespace>> MappedGetAllAsync()
        {

            IList<V1Namespace> namespaces = await GetAllAsync();
            var mappedns = _mapper.Map<List<TakaNamespace>>(namespaces);
            return mappedns;
        }

        public IList<TakaNamespace> GetMapped(string labelSelector)
        {
            var namespaces = _client.ListNamespace(labelSelector: labelSelector).Items;
            var mappedns = _mapper.Map<List<TakaNamespace>>(namespaces);
            return mappedns;
        }

        public async Task<IList<TakaNamespace>> GetMappedAsync(string labelSelector)
        {
            var namespaces = await _client.ListNamespaceAsync(labelSelector: labelSelector);
            var mappedns = _mapper.Map<List<TakaNamespace>>(namespaces.Items);
            return mappedns;
        }
    }
}