using System;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

namespace Taka.K8s.Tests
{
    public class NamespaceClientTests
    {
        private readonly ITestOutputHelper _output;

        public NamespaceClientTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void NamespaceClientShouldInitialize()
        {
            NamespaceClient nsClient = new NamespaceClient(StaticHelpers.GetK8sClient(),
            StaticHelpers.GetNamespaceClientLogger(),
            StaticHelpers.GetMapper());
            _output.WriteLine("Success");
            Console.WriteLine("Blahhh");
            Assert.NotNull(nsClient);

        }

        [Fact]
        public void NamespaceClientShouldReturnGetAll()
        {
            NamespaceClient nsClient = new NamespaceClient(StaticHelpers.GetK8sClient(),
                StaticHelpers.GetNamespaceClientLogger(),
                StaticHelpers.GetMapper());

            Assert.NotEmpty(nsClient.GetAll());
        }


        [Fact]
        public void NamespaceClientShouldReturnGetAllAsync()
        {
            NamespaceClient nsClient = new NamespaceClient(StaticHelpers.GetK8sClient(),
                StaticHelpers.GetNamespaceClientLogger(),
                StaticHelpers.GetMapper());
            var itemsTask = nsClient.GetAllAsync();
            itemsTask.Wait();

            Assert.NotEmpty(itemsTask.Result);
        }

        [Fact]
        public void ValidateNamespaceMapper()
        {
            var mapper = StaticHelpers.GetMapper();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void NamespaceClientShouldReturnMappedGetAll()
        {
            NamespaceClient nsClient = new NamespaceClient(StaticHelpers.GetK8sClient(),
                StaticHelpers.GetNamespaceClientLogger(),
                StaticHelpers.GetMapper());
            var items = nsClient.MappedGetAll();

            var message = String.Join(",", items.Select(p => p.Name));
            _output.WriteLine(message);

            Assert.NotEmpty(items);
        }


        [Fact]
        public void NamespaceClientShouldReturnMappedGetAllAsync()
        {
            NamespaceClient nsClient = new NamespaceClient(StaticHelpers.GetK8sClient(),
                StaticHelpers.GetNamespaceClientLogger(),
                StaticHelpers.GetMapper());
            var itemsTask = nsClient.MappedGetAllAsync();
            itemsTask.Wait();

            Assert.NotEmpty(itemsTask.Result);
        }


        [Fact]
        public void NamespaceClientShouldReturnGetMapped()
        {
            NamespaceClient nsClient = new NamespaceClient(StaticHelpers.GetK8sClient(),
                StaticHelpers.GetNamespaceClientLogger(),
                StaticHelpers.GetMapper());
            var mapped = nsClient.GetMapped("istio-injection=enabled");

            var message = String.Join(",", mapped.Select(p => p.Name));
            _output.WriteLine(message);

            Assert.NotEmpty(mapped);
        }


        [Fact]
        public void NamespaceClientShouldReturnGetMappedAsync()
        {
            NamespaceClient nsClient = new NamespaceClient(StaticHelpers.GetK8sClient(),
                StaticHelpers.GetNamespaceClientLogger(),
                StaticHelpers.GetMapper());
            var itemsTask = nsClient.GetMappedAsync("istio-injection=enabled");
            itemsTask.Wait();

            Assert.NotEmpty(itemsTask.Result);
        }

    }
}