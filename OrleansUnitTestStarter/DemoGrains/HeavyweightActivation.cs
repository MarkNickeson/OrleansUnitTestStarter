using DemoInterfacesAndTypes;
using Orleans.Placement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGrains
{
    [PreferLocalPlacement] // place the heavyweight activation on same silo as gateway
    public class HeavyweightActivation : Grain, IHeavyweightActivation
    {
        string? _pokedWith;
        string? _lastMessage;

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // simulate with a delay
            await Task.Delay(5000);
        }

        public Task PokeToActivate(string pokeWith)
        {
            _pokedWith = pokeWith;
            return Task.CompletedTask;
        }

        public Task<string?> GetPokedWith()
        {
            return Task.FromResult(_pokedWith);
        }

        public Task<string?> GetLastMessage()
        {
            return Task.FromResult(_lastMessage);
        }

        public async ValueTask HandleMessage(string message)
        {
            _lastMessage = message;
            // simulate 1000 ms message processing time
            await Task.Delay(1000);
        }
    }
}
