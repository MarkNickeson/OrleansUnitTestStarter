using DemoInterfacesAndTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePatternTests.CoHostedService
{
    public class CoHostedServiceTest
    {
        [Fact]
        public async Task Test()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.UseCoHostedService();
            builder.UseOrleans(builder => builder
                   .UseLocalhostClustering() // single silo cluster
               );

            var host = await builder.StartAsync();

            var client = host.Services.GetService<IClusterClient>();
            

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                IReadOptionsTest grain = client!.GetGrain<IReadOptionsTest>($"dummy{i+1}");
                var age = grain.GetGrainCollectionAge();                
            }
            await host.StopAsync();            
            Debug.WriteLine("Done");
        }
    }
}
