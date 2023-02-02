using DemoInterfacesAndTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGrains
{
    public class HeyCallMeBack : Grain, IHeyCallMeBack
    {
        public Task CallBackIn(IClientSideObserver callback, TimeSpan deltaT)
        {
            // kick off the callback, retaining the task 
            // in general: this works because the "work task" is not awaited
            var task = DoCallbackIn(callback, deltaT);
            return Task.CompletedTask;
        }

        public Task CallBackNTimes(IClientSideObserver callback, TimeSpan deltaT, int maxCount)
        {
            // kick off the callback, retaining the task 
            // in general: this works because the "work task" is not awaited
            var task = DoNCallbacks(callback, deltaT, maxCount);
            return Task.CompletedTask;
        }

        public async Task DoBlockingWait(TimeSpan waitTime)
        {
            await Task.Delay(waitTime);
        }

        async Task DoCallbackIn(IClientSideObserver callback, TimeSpan deltaT)
        {
            await Task.Yield();
            await Task.Delay(deltaT);
            await callback.Notify("yo");
        }

        async Task DoNCallbacks(IClientSideObserver callback, TimeSpan deltaT, int maxCount)
        {
            await Task.Yield();
            int curCount = 0;
            while (curCount < maxCount)
            {
                await Task.Delay(deltaT);
                await callback.Notify($"Callback {curCount++}");
            }
        }
    }
}
