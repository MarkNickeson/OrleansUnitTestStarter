using CommonInterfacesAndTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text;

namespace MinimumViableTests
{
    public class TwoSiloClusterTests : IAsyncLifetime
    {
        IHost? siloHost1;
        IHost? siloHost2;
        IHost? clientHost;
        IClusterClient? client;        

        public async Task InitializeAsync()
        {
            // NOTE:
            // - this setup will not work if concurrent unit tests is enabled
            // - This setup creates and tears down for every test so it may seem slow
            // - For better performance you can use IClassFixture<> or CollectionDefinitionAttribute
            siloHost1 = await Host.CreateDefaultBuilder()
                .UseOrleans(builder => builder
                    .UseLocalhostClustering(11110,30000,null)  // null == primary silo
                ).StartAsync();

            siloHost2 = await Host.CreateDefaultBuilder()
                .UseOrleans(builder => builder
                    .UseLocalhostClustering(11111, 30001, new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 11110))
                ).StartAsync();


            clientHost = await Host.CreateDefaultBuilder()
                .UseOrleansClient(builder => builder
                    .UseLocalhostClustering(new int[] { 30000, 30001 }) 
                ).StartAsync();

            client = clientHost.Services.GetRequiredService<IClusterClient>();
        }

        [Fact]
        public async Task Test1()
        {
            var sb = new StringBuilder();
            sb.AppendLine("******************************************************");
            for (int i = 0; i < 10; i++)
            {
                var testGrain = client!.GetGrain<IWhereAmI>($"test grain {i}");
                sb.AppendLine(await testGrain.GetSiloPlacement());
            }
            sb.AppendLine("******************************************************");

            Debug.WriteLine(sb.ToString());
        }

        public async Task DisposeAsync()
        {
            await clientHost!.StopAsync();
            await siloHost2!.StopAsync(); // have to shut down first because depends on silo 1 as primary
            await siloHost1!.StopAsync();
        }
    }
}