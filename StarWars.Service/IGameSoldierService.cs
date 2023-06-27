using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Service;

public interface IGameSoldierService : IService<GameSoldier>
{
    List<GameSoldier> GetPage(int gameId, Dictionary<string, StringValues> queryParams);
    int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams);
}