using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrleansCodeGen.Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePatterns
{
    public class SingleSiloWithClientFixture : IAsyncLifetime
    {
        IHost? _siloHost;
        IHost? _clientHost;
        IClusterClient? _client;

        public IClusterClient? Client => _client;

        public async Task InitializeAsync()
        {
            _siloHost = await Host.CreateDefaultBuilder()
               .UseOrleans(builder => builder
                   .UseLocalhostClustering() // single silo cluster
               ).StartAsync();

            _clientHost = await Host.CreateDefaultBuilder()
                .UseOrleansClient(builder => builder
                    .UseLocalhostClustering()
                ).StartAsync();

            _client = _clientHost.Services.GetRequiredService<IClusterClient>();
        }

        public async Task DisposeAsync()
        {
            if (_clientHost != null) await _clientHost!.StopAsync();
            if (_siloHost != null) await _siloHost!.StopAsync();
        }
    }
}
