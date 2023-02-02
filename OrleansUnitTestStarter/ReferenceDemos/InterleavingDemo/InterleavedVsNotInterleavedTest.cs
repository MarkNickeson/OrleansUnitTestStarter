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
    public class InterleavedVsNotInterleavedTests : IClassFixture<SingleSiloWithClientFixture>
    {
        readonly SingleSiloWithClientFixture _fixture;
        readonly IClusterClient _client;

        public InterleavedVsNotInterleavedTests(SingleSiloWithClientFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client!;
        }

        [Fact]
        public void InterleavedTest()
        {
            var delayBetween = 500; // milliseconds
            var maxEvents = 5;
            var sw = new Stopwatch();
            var countdownEvent = new CountdownEvent(maxEvents);

            // prepare an observer
            var observer = new ClientSideObserverRelay((payload) =>
            {
                countdownEvent.Signal();
            });

            var observerRef = _client.CreateObjectReference<IClientSideObserver>(observer);
            var testGrain = _client.GetGrain<IInterleavedTesting>("hey");

            // ready to test
            sw.Restart();
            var tasks = new List<Task>();
            // invoke in sequence, but each invoke within own background task
            for(int i=0;i<maxEvents;i++)
            {
                var backgroundTask = Task.Run(async () =>
                {
                    await testGrain.ExecuteInterleaved(observerRef, TimeSpan.FromMilliseconds(delayBetween), i + 1);// <-------- interleaved
                });
                tasks.Add(backgroundTask);
            }

            // wait for countdownEvent event to signal, indicating the callbacks have all been invoked
            countdownEvent.Wait();

            // check expected wait
            // - expect all calls to interleave because simulated delay yields. This means all should signal completion within approx delayBetween, NOT maxEvents*delayBetween
            sw.Stop();
            Assert.True(sw.ElapsedMilliseconds >= delayBetween);
            Assert.True(sw.ElapsedMilliseconds < delayBetween*2);
        }

        [Fact]
        public void NotInterleavedTest()
        {
            var delayBetween = 500; // milliseconds
            var maxEvents = 5;
            var sw = new Stopwatch();
            var countdownEvent = new CountdownEvent(maxEvents);

            // prepare an observer
            var observer = new ClientSideObserverRelay((payload) =>
            {
                countdownEvent.Signal();
            });

            var observerRef = _client.CreateObjectReference<IClientSideObserver>(observer);
            var testGrain = _client.GetGrain<IInterleavedTesting>("hey");

            // ready to test
            sw.Restart();
            var tasks = new List<Task>();
            // invoke in sequence, but each invoke within own background task
            for (int i = 0; i < maxEvents; i++)
            {
                var backgroundTask = Task.Run(async () =>
                {
                    await testGrain.ExecuteNonInterleaved(observerRef, TimeSpan.FromMilliseconds(delayBetween), i + 1);    // <-------- not interleaved
                });
                tasks.Add(backgroundTask);
            }

            // wait for countdownEvent event to signal, indicating the callbacks have all been invoked
            countdownEvent.Wait();

            // check expected wait
            // - expect calls to queue up in order because not interleaved. This means completion signal should take at least delayBetween*maxEvents
            sw.Stop();
            Assert.True(sw.ElapsedMilliseconds >= delayBetween*maxEvents);
        }
    }
}
