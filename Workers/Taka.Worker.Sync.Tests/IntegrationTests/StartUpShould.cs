using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Taka.Worker.Sync.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Taka.Worker.Sync.Tests.IntegrationTests
{

    [Collection("WebApplicationFactory")]
    public class StartUpShould
    {

        private WebApplicationFactory<Startup> factory;
        AuthTokenFixture authTokenFixture;
        private ITestOutputHelper output;

        public StartUpShould(CustomWebApplicationFactory factory, AuthTokenFixture authTokenFixture, ITestOutputHelper output)
        {
            this.factory = factory;

            this.output = output;
            this.authTokenFixture = authTokenFixture;
        }


        [Theory]
        [InlineData("/Health/IsAlive")]
        [InlineData("/Health/IsAliveAndWell")]
        public void Run(string url)
        {
            var client = factory.CreateClient();
            // client.DefaultRequestHeaders.Add("api-version", "1.0"); client.DefaultRequestHeaders.Add("Authorization", this.authTokenFixture.Token);
            client.DefaultRequestHeaders.Add("Internal", "true");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Act
            var responseTask = client.GetAsync(url);
            responseTask.Wait();
            var response = responseTask.Result;
            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}