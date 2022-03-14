namespace Example;

public interface IRepository
{
    List<Person> GetPersons();
}

public class Repository : IRepository
{
    public Repository()
    {
        Console.WriteLine("Initializing repository");
        Thread.Sleep(3000);
        Console.WriteLine("Repository initialized");
    }

    public List<Person> GetPersons()
    {
        return new List<Person>
        {
            new Person
            {
                Name = "Chip",
                Age = 31
            },
            new Person
            {
                Name = "Dale",
                Age = 32
            }
        };
    }
}
