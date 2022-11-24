using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesAndTypes
{
    public interface IWhereAmI : IGrainWithStringKey
    {
        Task<string> GetSiloPlacement();
    }
}
