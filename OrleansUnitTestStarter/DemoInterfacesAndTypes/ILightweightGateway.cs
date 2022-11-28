using Orleans.Concurrency;

namespace DemoInterfacesAndTypes
{
    public interface ILightweightGateway : IGrainWithStringKey
    {
        Task<IHeavyweightActivation> OvertActivation();
        
        [OneWay]
        ValueTask RelayMessage(string messageToRelay);
    }
}
