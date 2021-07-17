using System.Threading;
using System.Threading.Tasks;
using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Taka.K8s;
using Taka.Worker.Sync.Services;
using Xunit;

namespace Taka.Worker.Sync.Tests
{
    public class NamespaceWatcherTests
    {
        [Fact]
        public async Task TestExecutebyNewInstanceAsync()
        {
            NamespaceWatcher job = new NamespaceWatcher(GetKubernetesClient(), CreateLogger());
            var jobExecutionContext = new Mock<IJobExecutionContext>();
            // jobExecutionContext.Setup(p => p.Result()).Returns(new )
            // jobExecutionContext.Setup(p => p.Charge()).Returns(true)
            // await job.Execute(jobExecutionContext.Object);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            await job.StartAsync(token);
        }



        private ILogger<NamespaceWatcher> CreateLogger()
        {
            var serviceProvider = new ServiceCollection()
           .AddLogging()
           .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<NamespaceWatcher>();
            return logger;
        }


        public IKubernetesClient GetKubernetesClient()
        {
            return new KubernetesClientFromConfigFile();
        }
    }
}