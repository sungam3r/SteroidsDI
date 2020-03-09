namespace Example
{
    public interface IRepositoryFactory
    {
        // method name does not matter
        IRepository GetPersonsRepo();
    }
}
