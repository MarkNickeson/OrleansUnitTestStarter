using DemoGrains;
using DemoInterfacesAndTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
using SamplePatterns;

namespace ReferencePatterns.ReferenceSerializability
{
    public class ReferenceSerializabilityTests : IClassFixture<SingleSiloWithClientFixture>
    {
        readonly SingleSiloWithClientFixture _fixture;
        readonly IClusterClient _client;

        public ReferenceSerializabilityTests(SingleSiloWithClientFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client!;
        }

        [Fact]
        public async Task GrainReferenceFactoryPattern()
        {
            // This test demonstrates that grain references are serializable and that the
            // capability can be used in factory pattern

            string testState = Guid.NewGuid().ToString();
            string otherGrainKey = "the by reference grain";

            // get grain
            var creator = _client.GetGrain<IReferenceDemoGrain>("creator grain");
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
            var obsref = _client.CreateObjectReference<IClientSideObserver>(obs);

            var factory = _client.GetGrain<IObserverReferenceDemo>("factory");
            var refGrain = await factory.InitObserverFactory("ref grain holding observer", obsref);

            // now, poke the ref grain and expect notify callback with matching payload
            await refGrain.TestCallback(testPayload);

            Assert.Equal(testPayload!, obs.LastPayoad!);
        }
    }
}