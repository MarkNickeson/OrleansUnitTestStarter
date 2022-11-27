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
}