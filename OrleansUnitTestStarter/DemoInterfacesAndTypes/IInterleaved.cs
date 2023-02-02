using Orleans.Concurrency;

namespace DemoInterfacesAndTypes
{   
    public interface IInterleavedTesting : IGrainWithStringKey
    {
        [AlwaysInterleave]
        Task ExecuteInterleaved(IClientSideObserver observer, TimeSpan simulatedDelay, int sequence);
        
        Task ExecuteNonInterleaved(IClientSideObserver observer, TimeSpan simulatedDelay, int sequence);
    }    
}
