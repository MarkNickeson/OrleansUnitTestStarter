using DemoGrains;
using DemoInterfacesAndTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
using SamplePatterns;

namespace ReferencePatterns.ReferenceSerializability
{
    public class PassingGrainReferenceToOtherGrainTests : IClassFixture<SingleSiloWithClientFixture>
    {
        readonly SingleSiloWithClientFixture _fixture;
        readonly IClusterClient _client;

        public PassingGrainReferenceToOtherGrainTests(SingleSiloWithClientFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client!;
        }

        [Fact]
        public async Task GenerateReferenceViaCast()
        {
            // this test demonstrates a grain passing itself by reference to another grain

            string secret = Guid.NewGuid().ToString();
            string firstGrainKey = "first grain";
            string secondGrainKey = "second grain";

            // get first grain
            var first = _client.GetGrain<IPassingGrainReferenceOrigin>(firstGrainKey);
            // have first cache a secret and relay itself to second grain
            await first.SetSecretAndRelaySelfUsingCast(secret,secondGrainKey);

            // get second grain
            var second = _client.GetGrain<IPassingGrainReferenceRecovery>(secondGrainKey);
            // request secret from second, which it must do by using internal reference to grain 1
            var replaySecret = await second.GetSecretFromOrigin();

            // expect the secret to match
            Assert.Equal(replaySecret, secret);
        }

        [Fact]
        public async Task GenerateReferenceViaThis()
        {
            // this test demonstrates a grain passing itself by reference to another grain

            string secret = Guid.NewGuid().ToString();
            string firstGrainKey = "first grain";
            string secondGrainKey = "second grain";

            // get first grain
            var first = _client.GetGrain<IPassingGrainReferenceOrigin>(firstGrainKey);
            // have first cache a secret and relay itself to second grain
            await first.SetSecretAndRelaySelfUsingThis(secret, secondGrainKey);

            // get second grain
            var second = _client.GetGrain<IPassingGrainReferenceRecovery>(secondGrainKey);
            // request secret from second, which it must do by using internal reference to grain 1
            var replaySecret = await second.GetSecretFromOrigin();

            // expect the secret to match
            Assert.Equal(replaySecret, secret);
        }

        [Fact]
        public async Task GenerateReferenceViaAsReference()
        {
            // this test demonstrates a grain passing itself by reference to another grain

            string secret = Guid.NewGuid().ToString();
            string firstGrainKey = "first grain";
            string secondGrainKey = "second grain";

            // get first grain
            var first = _client.GetGrain<IPassingGrainReferenceOrigin>(firstGrainKey);
            // have first cache a secret and relay itself to second grain
            await first.SetSecretAndRelaySelfUsingAsReference(secret, secondGrainKey);

            // get second grain
            var second = _client.GetGrain<IPassingGrainReferenceRecovery>(secondGrainKey);
            // request secret from second, which it must do by using internal reference to grain 1
            var replaySecret = await second.GetSecretFromOrigin();

            // expect the secret to match
            Assert.Equal(replaySecret, secret);
        }
    }
}