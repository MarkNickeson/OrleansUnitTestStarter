namespace DemoInterfacesAndTypes
{
    public interface IReferenceDemoGrain : IGrainWithStringKey
    {
        Task SetTestState(string? state);

        Task<string?> GetTestState();

        Task<IReferenceDemoGrain> InitReference(string key, string? testState);
    }
}