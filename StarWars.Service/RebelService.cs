using StarWars.Model;

namespace StarWars.Service;

public class RebelService : Service<Rebel>, IRebelService
{

    public RebelService(StarWarsDbContext context) : base(context)
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