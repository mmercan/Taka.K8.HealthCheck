using System;
using Xunit;
using Xunit.Abstractions;

namespace Taka.K8s.Tests
{
    public class NamespaceWatchClientTests
    {
        private readonly ITestOutputHelper _output;

        public NamespaceWatchClientTests(ITestOutputHelper output) => _output = output;


        [Fact]
        public void NamespaceWatchClientShouldInitialize()
        {
            //Given
            NamespaceWatchClient watchClient = new NamespaceWatchClient(StaticHelpers.GetK8sClient(),
                    StaticHelpers.GetNamespaceClientLogger(),
                    StaticHelpers.GetMapper());

            // watchClient.WatchTriggered += new EventHandler(delegate (Object o, EventArgs a) 
            // {
            //     //snip
            // });

            _output.WriteLine("events waiting");
            watchClient.WatchTriggered += (o, e) =>
            {
                _output.WriteLine("event triggered");
            };

            var taskwatch = watchClient.StartAsync();
            taskwatch.Wait();
            // var waittask = System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(1));
            // waittask.Wait();
            //When

            //Then
        }

    }
}