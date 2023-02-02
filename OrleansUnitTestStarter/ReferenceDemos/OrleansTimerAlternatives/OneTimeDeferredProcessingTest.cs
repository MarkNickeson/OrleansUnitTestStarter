using DemoGrains;
using DemoInterfacesAndTypes;
using SamplePatterns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePatternTests.OrleansTimerAlternatives
{
    public class OneTimeDeferredProcessingTests : IClassFixture<SingleSiloWithClientFixture>
    {
        readonly SingleSiloWithClientFixture _fixture;
        readonly IClusterClient _client;

        public OneTimeDeferredProcessingTests(SingleSiloWithClientFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client!;
        }

        [Fact]
        public async Task Test()
        {
            // prepare a stopwatch
            var sw = new Stopwatch();
            var autoResetEvent = new AutoResetEvent(false);

            // prepare an observer
            var observer = new ClientSideObserverRelay((payload) =>
            {
                autoResetEvent.Set();
            });

            var observerRef = _client.CreateObjectReference<IClientSideObserver>(observer);
            var testGrain = _client.GetGrain<IHeyCallMeBack>("hey");

            // ready to test
            sw.Restart();
            await testGrain.CallBackIn(observerRef, TimeSpan.FromMilliseconds(2000));
            // wait for autoreset event to signal, but not more than 2 seconds
            if (!autoResetEvent.WaitOne(2000))
            {
                Assert.Fail("Wait timeout");
            }
            sw.Stop();
            // expect elapsed time to be at least 2000ms
            Assert.True(sw.ElapsedMilliseconds >= 2000.0);
        }
    }
}
