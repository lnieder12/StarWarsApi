using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Service;

public interface IRoundService : IService<Round>
{
    Round AddRound(int idAtt, int idDef, Game game);
    void PostAll(List<Round> rounds);
    Round GetInclude(int id);
    List<Round> GetPage(int gameId, Dictionary<string, StringValues> queryParams);
    int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams);
    void PatchRoundsDamage(Soldier attacker);
}