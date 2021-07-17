using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Taka.Worker.Sync.Tests.Helpers
{
    [CollectionDefinition("WebApplicationFactory")]
    public class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactory<Startup>>, ICollectionFixture<AuthTokenFixture>, ICollectionFixture<CustomWebApplicationFactory>
    {


    }
}