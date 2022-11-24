using CommonInterfacesAndTypes;
using Orleans.Runtime;

namespace GrainImplementations
{
    public class PocoFoo : IGrainBase, IFoo
    {
        public IGrainContext GrainContext { get; }

        public PocoFoo(IGrainContext grainContext)
        {
            GrainContext = grainContext;
        }

        public Task<double> Add(double a, double b)
        {
            return Task.FromResult(a + b);
        }
    }
}