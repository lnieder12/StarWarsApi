using Microsoft.Extensions.Primitives;
using StarWars.Repository;

namespace StarWars.Service;

public class GamePageService<T> : Service<T> where T : class
{
    public GamePageRepository<T> PageRepo;


    public GamePageService(IRepository<T> repo, GamePageRepository<T> pageRepo) : base(repo)
    {
        PageRepo = pageRepo;
    }

    public virtual List<T> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return PageRepo.GetPage(gameId, queryParams);
    }

    public virtual int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return PageRepo.GetPageCount(gameId, queryParams);
    }

}