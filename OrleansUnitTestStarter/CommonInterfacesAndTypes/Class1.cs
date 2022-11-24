namespace CommonInterfacesAndTypes
{
    public interface IFoo : IGrainWithStringKey
    {
        Task<double> Add(double a, double b);
    }
}