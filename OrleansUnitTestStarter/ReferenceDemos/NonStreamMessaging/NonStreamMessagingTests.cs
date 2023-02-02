using DemoInterfacesAndTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePatterns.NonStreamMessaging
{
    public class NonStreamMessagingTests : IClassFixture<SingleSiloWithClientFixture>
    {
        readonly SingleSiloWithClientFixture _fixture;
        readonly IClusterClient _client;

        public NonStreamMessagingTests(SingleSiloWithClientFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client!;
        }

        [Fact]
        public async Task DemonstrateThatDirectActivationIsExpensive()
        {
            var heavy = _client.GetGrain<IHeavyweightActivation>("test key 1");
            var sw = Stopwatch.StartNew();
            await heavy.PokeToActivate("activated directly");
            sw.Stop();

            // expect activation to have taken at least 5000ms, which is hard wired in heavy.OnActivateAsync
            Assert.True(sw.ElapsedMilliseconds >= 5000.0);

            // verify the poked was retained
            var rval = await heavy.GetPokedWith();
            Assert.Equal("activated directly", rval);
        }

        [Fact]
        public async Task DemonstrateOvertActivationViaGateway()
        {
            var light = _client.GetGrain<ILightweightGateway>("test key 2");

            var sw = Stopwatch.StartNew();
            var heavy = await light.OvertActivation();
            sw.Stop();

            // expect overt activation to have taken at least 5000ms because that is how long heavy takes to activate
            Assert.True(sw.ElapsedMilliseconds >= 5000.0);

            // also verify that heavy was activate by poking it with lightweight
            var rval = await heavy.GetPokedWith();
            Assert.Equal("poked by lightweight to activate", rval);
        }

        [Fact]
        public async Task DemonstrateGatewayRelayDoesNotActivateHeavy()
        {
            var light = _client.GetGrain<ILightweightGateway>("test key 3");
            await light.RelayMessage("Test message");

            // directly resolve and interogate heavy
            var heavy = _client.GetGrain<IHeavyweightActivation>("test key 3");
            var sw = Stopwatch.StartNew();
            var pokedWith = await heavy.GetPokedWith();
            sw.Stop();

            // expect not poked with anything
            Assert.Null(pokedWith);

            // expect heavy activation to have taken at least 5000ms, which is hard wired in heavy.OnActivateAsync
            Assert.True(sw.ElapsedMilliseconds >= 5000.0);
        }

        [Fact]
        public async Task DemonstrateSingleRelayViaGateway()
        {
            var light = _client.GetGrain<ILightweightGateway>("test key 4");
            var heavy = await light.OvertActivation(); // will have high activation cost

            var sw = Stopwatch.StartNew();
            await light.RelayMessage("Test message");
            sw.Stop();

            // expect relay time to be <<< 1000 ms which is the simulated processing time within HandleMessage
            // eg light.RelayMessage uses one way to heavy.HandleMessage, so even though HandleMessage takes 1000ms to process, the relay does not have to wait
            Assert.True(sw.ElapsedMilliseconds < 1000.0);

            // also, it takes time for heavy to pick up the relayed message because all relay does is enqueue, so poll last message until not null
            var lastMessage = await heavy.GetLastMessage();
            while(lastMessage==null)
            {
                await Task.Delay(100);
                lastMessage = await heavy.GetLastMessage();
            }
            Assert.Equal("Test message", lastMessage);
        }
    }
}
