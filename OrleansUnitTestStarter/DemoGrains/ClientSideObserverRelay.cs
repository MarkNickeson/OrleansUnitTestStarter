using DemoInterfacesAndTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGrains
{
    public class ClientSideObserverRelay : IClientSideObserver
    {
        Action<string?> _relayTo;

        public ClientSideObserverRelay(Action<string?> relayTo)
        {
            _relayTo = relayTo;
        }

        public ValueTask Notify(string? payload)
        {
            if (_relayTo != null)
            {
                _relayTo(payload);
            }
            return default(ValueTask);
        }
    }
}
