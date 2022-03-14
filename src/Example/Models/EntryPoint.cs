namespace Example;

public interface IEntryPoint
{
    int DoSomethingImportant();
}

public class EntryPoint : IEntryPoint
{
    //private IRepository _repository; - this is an example of what you wanted to do but got the error below
    private readonly Defer<IRepository> _repository1;
    private readonly Func<IRepository> _repository2;
    private readonly IRepositoryFactory _repoFactory;

    public EntryPoint(
        // Error while validating the service descriptor 'ServiceType: Example.IEntryPoint Lifetime: Singleton ImplementationType: Example.EntryPoint':
        // Cannot consume scoped service 'Example.IRepository' from singleton 'Example.IEntryPoint'.
        //IRepository repository
        Defer<IRepository> repository1,
        Func<IRepository> repository2,
        IRepositoryFactory repoFactory)
    {
        _repository1 = repository1;
        _repository2 = repository2;
        _repoFactory = repoFactory;
    }

    public int DoSomethingImportant()
    {
        // All 3 APIs return the same instance
        var repo1 = _repository1.Value;
        var repo2 = _repository2();
        var repo3 = _repoFactory.GetPersonsRepo();

        if (!ReferenceEquals(repo1, repo2) || !ReferenceEquals(repo1, repo3))
            throw new InvalidOperationException();

        var persons = repo1.GetPersons();

        foreach (var person in persons)
        {
            Console.WriteLine($"{person.Name} ({person.Age})");
        }

        return persons.Count;
    }
}
