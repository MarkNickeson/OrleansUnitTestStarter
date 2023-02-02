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
            var testGrain = _client.GetGrain<IHeyCallMeBack>(Guid.NewGuid().ToString());

            // ready to test
            sw.Restart();
            await testGrain.CallBackIn(observerRef, TimeSpan.FromMilliseconds(2000));
            // wait for autoreset event to signal, but not more than 2 seconds
            if (!autoResetEvent.WaitOne(2500))
            {
                Assert.Fail("Wait timeout");
            }
            sw.Stop();
            // expect elapsed time to be at least 2000ms
            Assert.True(sw.ElapsedMilliseconds >= 2000.0);
        }

        [Fact(Skip="Known to fail because the calls DO interleave")]
        public async Task NotInterleavedTest()
        {
            // prepare a stopwatch and trigger event
            var sw = new Stopwatch();
            var autoResetEvent = new AutoResetEvent(false);

            // prepare an observer
            var observer = new ClientSideObserverRelay((payload) =>
            {
                autoResetEvent.Set();
            });

            var observerRef = _client.CreateObjectReference<IClientSideObserver>(observer);
            var testGrain = _client.GetGrain<IHeyCallMeBack>(Guid.NewGuid().ToString());

            // ready to test
            sw.Restart();
            // kick off the deferred callback
            await testGrain.CallBackIn(observerRef, TimeSpan.FromMilliseconds(2000));

            // start a background task to wait for callback to signal
            var backgroundTask = Task.Run(() =>
            {
                if (autoResetEvent.WaitOne(5000))
                {
                    // callback has signalled. Stop the timer so the elapsed time can be assessed
                    sw.Stop();
                }
            });

            // pause for 1500ms, just less than expected callback time
            await Task.Delay(1500);

            // now invoke blocking delay. The idea is to get in front of the scheduled callback and.
            // If the call is non-interleaved, the effect should be to cause the callback to be delayed by approx "blocking wait time"-500ms
            await testGrain.DoBlockingWait(TimeSpan.FromMilliseconds(2500));

            // when the blocking wait is over, await the background task
            await backgroundTask;
           
            // expect elapsed time to be at least 4000ms (1500ms + 2500ms)
            Assert.True(sw.ElapsedMilliseconds >= 4000.0);
        }
    }
}
