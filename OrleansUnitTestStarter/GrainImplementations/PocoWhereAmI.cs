using CommonInterfacesAndTypes;
using Orleans.Runtime;

namespace GrainImplementations
{
    public class PocoWhereAmI : IGrainBase, IWhereAmI
    {
        public IGrainContext GrainContext { get; }
        public IGrainRuntime GrainRuntime  { get; }

        public PocoWhereAmI(IGrainContext grainContext, IGrainRuntime grainRuntime)
        {
            GrainContext = grainContext;
            GrainRuntime = grainRuntime;
        }

        public Task<string> GetSiloPlacement()
        {
            return Task.FromResult(GrainRuntime.SiloIdentity);
        }
    }
}