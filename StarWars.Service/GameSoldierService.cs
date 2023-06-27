using Microsoft.Extensions.Primitives;
using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class GameSoldierService : GamePageService<GameSoldier>, IGameSoldierService
{

    private readonly GameSoldierRepository _gsRepo;


    public GameSoldierService(IRepository<GameSoldier> repo, GamePageRepository<GameSoldier> pageRepo, GameSoldierRepository gsRepo) : base(repo, pageRepo)
    {
        _gsRepo = gsRepo;
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