using System;
using k8s;
using k8s.Exceptions;
using Xunit;

namespace Taka.K8s.Tests
{
    public class KubernetesClientTests
    {

        [Fact]
        public void KubernetesClientFromConfigFileShouldReturnClient()
        {
            Kubernetes _client = new KubernetesClientFromConfigFile().Client;
            Assert.NotNull(_client);
        }

        [Fact]
        public void KubernetesClientInClusterConfigShouldThrowException()
        {
            Assert.Throws<KubeConfigException>(() => { Kubernetes _client = new KubernetesClientInClusterConfig().Client; });
        }

    }
}