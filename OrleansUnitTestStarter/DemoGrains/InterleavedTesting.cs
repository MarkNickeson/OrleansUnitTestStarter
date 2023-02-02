using DemoInterfacesAndTypes;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGrains
{
    internal class InterleavedTesting : Grain, IInterleavedTesting
    {
        public async Task ExecuteInterleaved(IClientSideObserver observer, TimeSpan simulatedDelay, int sequenceID)
        {
            await Task.Delay(simulatedDelay);
            await observer.Notify($"{sequenceID} ExecuteInterleaved simulated delay completed");
        }

        public async Task ExecuteNonInterleaved(IClientSideObserver observer, TimeSpan simulatedDelay, int sequenceID)
        {
            await Task.Delay(simulatedDelay);
            await observer.Notify($"{sequenceID} ExecuteNonInterleaved simulated delay completed");
        }
    }
}
