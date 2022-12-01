using DemoInterfacesAndTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGrains
{
    public  class ObserverReferenceDemo : Grain, IObserverReferenceDemo
    {
        IClientSideObserver? _observer;

        public async Task<IObserverReferenceDemo> InitObserverFactory(string targetKey, IClientSideObserver observer)
        {
            var refGrain = GrainFactory.GetGrain<IObserverReferenceDemo>(targetKey);
            await refGrain.SetObserver(observer);
            return refGrain;
        }

        public async Task TestCallback(string payload)
        {
            if (_observer != null)
            {
                await _observer.Notify(payload);
            }
            else
            {
                throw new ApplicationException();
            }
        }

        public Task SetObserver(IClientSideObserver observer)
        {
            _observer = observer;
            return Task.CompletedTask;
        }
    }

    public class ObserverReferenceEqualityDemo : Grain, IObserverReferenceEqualityDemo
    {
        IClientSideObserver? _observer;

        public Task Once(IClientSideObserver observer)
        {
            _observer = observer;
            return Task.CompletedTask;
        }

        public Task<bool> TwiceEqualitySign(IClientSideObserver observer)
        {
            return Task.FromResult(_observer == observer);
        }

        public Task<bool> TwiceEqualsMethod(IClientSideObserver observer)
        {
            return Task.FromResult(observer.Equals(_observer));
        }
    }
}
