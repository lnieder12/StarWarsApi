using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class ServiceGameSoldier : Service<GameSoldier>
{

    private readonly GameSoldierRepository _gsRepo;

    public ServiceGameSoldier(StarWarsDbContext context) : base(context)
    {
        this._gsRepo = new GameSoldierRepository(context);
    }

    public GameSoldier Get(int gameId, int soldierId)
    {
        return this._gsRepo.Get(gameId, soldierId);
    }


}