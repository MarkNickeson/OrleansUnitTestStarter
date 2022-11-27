using DemoInterfacesAndTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGrains
{
    public class ClientSideObserver : IClientSideObserver
    {
        public string? LastPayoad { get; set; }

        public ValueTask Notify(string? payload)
        {
            LastPayoad = payload;
            return default(ValueTask);
        }
    }
}
