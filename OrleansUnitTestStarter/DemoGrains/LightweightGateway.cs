using DemoInterfacesAndTypes;

namespace DemoGrains
{
    public class LightweightGateway : Grain, ILightweightGateway
    {
        IHeavyweightActivation? _heavyweight;

        public async Task<IHeavyweightActivation> OvertActivation()
        {
            if (_heavyweight == null)
            {
                // first over activation
                var key = this.GetPrimaryKeyString();
                _heavyweight = GrainFactory.GetGrain<IHeavyweightActivation>(key); // heavy weight assumed to have same key as gateway
                await _heavyweight.PokeToActivate("poked by lightweight to activate");
                return _heavyweight;
            }
            else
            {
                throw new ApplicationException("Duplicate over activation");
            }
        }

        public async ValueTask RelayMessage(string messageToRelay)
        {
            // only relay if over activation has been called, otherwise drop on floor
            if (_heavyweight != null)
            {
                await _heavyweight.HandleMessage(messageToRelay);
            }
        }
    }
}
