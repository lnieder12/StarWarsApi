using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class RebelService : Service<Rebel>, IRebelService
{
    public RebelService(IRepository<Rebel> repo) : base(repo)
    {
    }

    public Rebel CreateRandom(int number)
    {
        var rebel = new Rebel();

        var random = new Random();

        rebel.Attack = random.Next(100, 500);
        rebel.MaxHealth = random.Next(1000, 2000);
        rebel.Name = "REB-" + number;

        return this.Add(rebel);
    }

}