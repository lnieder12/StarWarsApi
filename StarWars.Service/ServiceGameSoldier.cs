using Microsoft.Extensions.Primitives;
using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class ServiceGameSoldier : GamePageService<GameSoldier>
{

    private readonly GameSoldierRepository _gsRepo;

    public ServiceGameSoldier(StarWarsDbContext context) : base(context)
    {
        _gsRepo = new GameSoldierRepository(context);
    }

    public override List<GameSoldier> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return _gsRepo.GetPage(gameId, queryParams);
    }

    public override int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return _gsRepo.GetPageCount(gameId, queryParams);
    }

}