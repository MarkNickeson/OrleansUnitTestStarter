using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoInterfacesAndTypes
{
    public interface IReadOptionsTest : IGrainWithStringKey
    {
        Task<TimeSpan> GetGrainCollectionAge();        
    }
}
