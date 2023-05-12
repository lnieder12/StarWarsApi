using StarWars.Controllers;
using StarWars.Model;

namespace StarWars.Service;

public class ServiceGameSoldier : Service<GameSoldier>
{

    private GameSoldierRepository gsRepo;

    public ServiceGameSoldier(StarWarsDbContext context) : base(context)
    {
        this.gsRepo = new GameSoldierRepository(context);
    }

    public GameSoldier Get(int gameId, int soldierId)
    {
        return this.gsRepo.Get(gameId, soldierId);
    }


}