using DemoInterfacesAndTypes;

namespace DemoGrains
{
    public class PassingGrainReferenceOriginGrain : Grain, IPassingGrainReferenceOrigin
    {
        string? _secret;

        public Task<string?> GetSecret()
        {
            return Task.FromResult(_secret);
        }

        public async Task SetSecretAndRelaySelfUsingCast(string secret, string secondGrainKey)
        {
            _secret = secret;
            // create ref to second
            var second = GrainFactory.GetGrain<IPassingGrainReferenceRecovery>(secondGrainKey);
            // pass self to second using CAST
            await second.SetOriginGrain(this.Cast<IPassingGrainReferenceOrigin>());
        }

        public async Task SetSecretAndRelaySelfUsingThis(string secret, string secondGrainKey)
        {
            _secret = secret;
            // create ref to second
            var second = GrainFactory.GetGrain<IPassingGrainReferenceRecovery>(secondGrainKey);
            // pass self to second using CAST
            await second.SetOriginGrain(this);
        }

        public async Task SetSecretAndRelaySelfUsingAsReference(string secret, string secondGrainKey)
        {
            _secret = secret;
            // create ref to second
            var second = GrainFactory.GetGrain<IPassingGrainReferenceRecovery>(secondGrainKey);
            // pass self to second using CAST
            await second.SetOriginGrain(this.AsReference<IPassingGrainReferenceOrigin>());
        }
    }

    public class PassingGrainReferenceRecoveryGrain : Grain, IPassingGrainReferenceRecovery
    {
        IPassingGrainReferenceOrigin? _originReference;

        public Task SetOriginGrain(IPassingGrainReferenceOrigin originReference)
        {
            _originReference = originReference;
            return Task.CompletedTask;
        }

        public async Task<string?> GetSecretFromOrigin()
        {
            if (_originReference != null)
            {
                return await _originReference.GetSecret();
            }
            else
            {
                throw new ApplicationException();
            }
        }
    }
}