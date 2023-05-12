using Microsoft.Extensions.Primitives;
using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class GamePageService<T> : Service<T> where T : class
{
    public GamePageRepository<T> PageRepo;
    public GamePageService(StarWarsDbContext context) : base(context)
    {
        PageRepo = new GamePageRepository<T>(context);
    }

    public List<T> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return PageRepo.GetPage(gameId, queryParams);
    }

    public int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return PageRepo.GetPageCount(gameId, queryParams);
    }

}