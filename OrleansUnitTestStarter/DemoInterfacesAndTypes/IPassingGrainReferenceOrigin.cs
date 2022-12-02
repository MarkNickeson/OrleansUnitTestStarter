using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoInterfacesAndTypes
{
    public interface IPassingGrainReferenceOrigin : IGrainWithStringKey
    {
        Task<string?> GetSecret();
        Task SetSecretAndRelaySelfUsingCast(string secret, string secondGrainKey);
        Task SetSecretAndRelaySelfUsingThis(string secret, string secondGrainKey);
        Task SetSecretAndRelaySelfUsingAsReference(string secret, string secondGrainKey);   
    }

    public interface IPassingGrainReferenceRecovery : IGrainWithStringKey
    {
        Task SetOriginGrain(IPassingGrainReferenceOrigin originReference);
        Task<string?> GetSecretFromOrigin();
    }
}
