using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Service;

public interface IGameService : IService<Game>
{
    Game CreateGame(int rebels, int empires, int nbRounds);
    Game CreateSelectedGame(RebelsEmpires soldiers, int nbRounds);
    Game GetIncludeAll(int id);
    Game GetIncludeSoldiers(int id);
    Game GetIncludeRounds(int id);
    Soldier AddSoldier(int id, int soldierId);
    Soldier PatchSoldier(int id, int soldierId, JsonPatchDocument<Soldier> patch);
    void AddRound(Game game, Round round);
    List<Round> GetRounds(int id);
    int NbRounds(int id);
    bool SoldierInGame(int id, int soldierId);
    List<GameSoldier> GetGs(int id);
    List<GameSoldier> GetGs<T>(Game game) where T : Soldier;
    List<Soldier> GetSoldiers(int id);
    List<Soldier> GsToSoldier(List<GameSoldier> gameSoldiers);
    List<Rebel> GetRebels(int id);
    List<Empire> GetEmpires(int id);
    List<GameSoldier> FilterValidSoldiers(List<GameSoldier> soldiers);
    GameSoldier GetRandom<T>(Game game) where T : Soldier;
    List<GameSoldier> GetValid<T>(Game game) where T : Soldier;
    int NbValidSoldier<T>(Game game) where T : Soldier;
    int GetNbValidSoldier<T>(int id) where T : Soldier;
    Round DoFight(Game game);
    Round Fight(int id);
    SoldierScore GetSoldierScore(GameSoldier gs);
    List<SoldierScore> GetSoldierScoresPage(int id, Dictionary<string, StringValues> queryParams);
    List<Round> GetRoundsPage(int id, Dictionary<string, StringValues> queryParams);
    int GetScoresFilteredCount(int id, Dictionary<string, StringValues> queryParams);
    int GetRoundsFilteredCount(int id, Dictionary<string, StringValues> queryParams);
    int TeamScore<T>(int id) where T : Soldier;
    List<Round> MultipleFights(int id, int nb);
    bool EnoughSoldiers(int id);
    string WinnerTeam(int id);
}