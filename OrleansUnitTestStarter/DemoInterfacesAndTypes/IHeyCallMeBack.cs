using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoInterfacesAndTypes
{
    public interface IHeyCallMeBack : IGrainWithStringKey
    {
        Task CallBackIn(IClientSideObserver callback, TimeSpan deltaT);

        Task CallBackNTimes(IClientSideObserver callback, TimeSpan deltaT, int maxCount);

        Task DoBlockingWait(TimeSpan waitTime);
    }
}
