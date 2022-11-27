using DemoGrains;
using DemoInterfacesAndTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;

namespace ReferenceDemos
{
    public class ReferenceTests : IAsyncLifetime
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
        public async Task GrainReferenceFactoryPattern()
        {            
            // This test demonstrates that grain references are serializable and that the
            // capability can be used in factory pattern

            string testState = Guid.NewGuid().ToString();
            string otherGrainKey = "the by reference grain";

            // get grain
            var creator = client!.GetGrain<IReferenceDemoGrain>("creator grain");
            // use grain to obtain a reference to another grain of the same type
            var refGrain = await creator.InitReference(otherGrainKey, testState);
            // read the state from reference grain
            var refGrainState = await refGrain.GetTestState();
            // confirm returned reference grain state matches state passed to init 
            Assert.Equal(testState, refGrainState);
        }

        [Fact]
        public async Task GrainObserverReferenceMobility()
        {
            // this test demonstrates that client side grain observer references are serializable
            // which allows a factory grain to configure another grain with the observer reference
            // and the callback works as expected.
            var testPayload = Guid.NewGuid().ToString();

            // prepare observer
            var obs = new ClientSideObserver();
            var obsref = client!.CreateObjectReference<IClientSideObserver>(obs);

            var factory = client.GetGrain<IObserverReferenceDemo>("factory");
            var refGrain = await factory.InitObserverFactory("ref grain holding observer", obsref);

            // now, poke the ref grain and expect notify callback with matching payload
            await refGrain.TestCallback(testPayload);

            Assert.Equal(testPayload!, obs.LastPayoad!);
        }

        public async Task DisposeAsync()
        {
            await clientHost!.StopAsync();
            await siloHost!.StopAsync();
        }
    }
}