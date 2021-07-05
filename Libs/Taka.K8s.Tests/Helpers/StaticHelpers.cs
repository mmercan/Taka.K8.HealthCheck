using AutoMapper;
using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Taka.K8s.Tests
{
    public static class StaticHelpers
    {

        static KubernetesClientFromConfigFile k8 = new KubernetesClientFromConfigFile();
        static MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<K8Mapper>());
        public static Kubernetes GetK8sClient()
        {
            return k8.Client;
        }

        public static IMapper GetMapper()
        {
            // config.AssertConfigurationIsValid();
            var mapper = config.CreateMapper();
            return mapper;
        }

        public static ILogger<Taka.K8s.NamespaceClient> GetNamespaceClientLogger()
        {
            var serviceProvider = new ServiceCollection()
           .AddLogging()
           .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<Taka.K8s.NamespaceClient>();
            return logger;
        }


    }
}