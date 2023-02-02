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
    public class ManyTimeDeferredProcessingTests : IClassFixture<SingleSiloWithClientFixture>
    {
        readonly SingleSiloWithClientFixture _fixture;
        readonly IClusterClient _client;

        public ManyTimeDeferredProcessingTests(SingleSiloWithClientFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client!;
        }

        [Fact]
        public async Task Test()
        {
            int testCallbackCount = 5;
            double period = 500.0; // milliseconds

            // prepare a stopwatch
            var sw = new Stopwatch();
            var countdownEvent = new CountdownEvent(testCallbackCount);

            // prepare an observer
            var observer = new ClientSideObserverRelay((payload) =>
            {
                countdownEvent.Signal();
            });

            var observerRef = _client.CreateObjectReference<IClientSideObserver>(observer);
            var testGrain = _client.GetGrain<IHeyCallMeBack>("hey");

            // ready to test
            sw.Restart();
            await testGrain.CallBackNTimes(observerRef, TimeSpan.FromMilliseconds(period), testCallbackCount);

            // wait for countdownEvent event to signal but not more than period*(testCallbackCount+1)
            var maxWait = period * (testCallbackCount + 1);
            if (!countdownEvent.Wait((int)maxWait))
            {
                Assert.Fail("Wait timeout");
            }

            // check expected wait
            sw.Stop();
            var expectedWait = period * testCallbackCount;
            Assert.True(sw.ElapsedMilliseconds >= expectedWait);
        }
    }
}
