using DemoInterfacesAndTypes;

namespace DemoGrains
{
    public class ReferenceGrainDemo : Grain, IReferenceDemoGrain
    {
        string? _state;

        public Task SetTestState(string? state)
        {
            _state = state;
            return Task.CompletedTask;
        }

        public Task<string?> GetTestState()
        {
            return Task.FromResult(_state);
        }

        public async Task<IReferenceDemoGrain> InitReference(string key, string? testState)
        {
            var testRef = this.GrainFactory.GetGrain<IReferenceDemoGrain>(key);
            await testRef.SetTestState(testState);
            return testRef;
        }
    }
}