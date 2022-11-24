using CommonInterfacesAndTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MainTests
{
    public class FirstTest : IAsyncLifetime
    {
        IHost? siloHost;
        IHost? clientHost;
        IClusterClient? client;        

        public async Task InitializeAsync()
        {
            // NOTE:
            // - this setup will not work if concurrent unit tests is enabled
            // - This setup creates and tears down for every test so it may seem slow
            // - For better performance you can use IClassFixture<> or CollectionDefinitionAttribute
            siloHost = await Host.CreateDefaultBuilder()
                .UseOrleans(builder => builder
                    .UseLocalhostClustering() // single silo cluster
                ).StartAsync();

            clientHost = await Host.CreateDefaultBuilder()
                .UseOrleansClient(builder => builder
                    .UseLocalhostClustering() 
                ).StartAsync();

            client = clientHost.Services.GetRequiredService<IClusterClient>();
        }

        [Fact]
        public async Task Test1()
        {
            var testGrain = client!.GetGrain<IFoo>("test grain id");
            var sum = await testGrain.Add(1.0, 2.0);
            Assert.Equal(3.0, sum);
        }

        public async Task DisposeAsync()
        {
            await clientHost!.StopAsync();
            await siloHost!.StopAsync();
        }
    }
}