namespace DemoInterfacesAndTypes
{
    public interface IClientSideObserver : IGrainObserver
    {
        ValueTask Notify(string payload);
    }

    public interface IObserverReferenceDemo : IGrainWithStringKey
    {
        Task<IObserverReferenceDemo> InitObserverFactory(string targetKey, IClientSideObserver observer);

        Task TestCallback(string payload);

        Task SetObserver(IClientSideObserver observer);
    }

    public interface IObserverReferenceEqualityDemo : IGrainWithStringKey
    {
        Task Once(IClientSideObserver observer);
        Task<bool> TwiceEqualitySign(IClientSideObserver observer);
        Task<bool> TwiceEqualsMethod(IClientSideObserver observer);
    }
}