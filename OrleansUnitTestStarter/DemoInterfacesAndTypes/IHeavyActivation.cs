using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoInterfacesAndTypes
{
    public interface IHeavyweightActivation : IGrainWithStringKey
    {
        Task PokeToActivate(string pokedWith);

        [OneWay]
        ValueTask HandleMessage(string message);

        Task<string?> GetPokedWith();

        Task<string?> GetLastMessage();
    }
}
