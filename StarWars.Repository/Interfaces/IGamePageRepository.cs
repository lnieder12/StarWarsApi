using Microsoft.Extensions.Primitives;

namespace StarWars.Repository.Interfaces;

public interface IGamePageRepository<T> : IRepository<T> where T : class
{
    IQueryable<T> GetQueryFilteredSorted(int gameId, Dictionary<string, StringValues> queryParams);
    int GetPageCount(int gameId, Dictionary<string, StringValues> queryParams);
    List<T> GetPage(int gameId, Dictionary<string, StringValues> queryParams);
}