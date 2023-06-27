using StarWars.Model;

namespace StarWars.Repository;

public interface IGameRepository : IRepository<Game>
{
    Game GetIncludeRounds(int id);
    void SaveGame(Game game);
    Game GetIncludeSoldiers(int id);
    Game GetIncludeAll(int id);
    new List<Game> GetAll();
}