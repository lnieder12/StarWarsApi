using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Repository;

public class FilterPageRepository<T> : Repository<T> where T : class
{
    public FilterPageRepository(StarWarsDbContext ctx) : base(ctx)
    {
    }

    public virtual IQueryable<T> GetQueryFilteredSorted(int gameId, Dictionary<string, StringValues> queryParams)
    {
        var db = Ctx.Set<T>();

        var query = db.AddFiltersSorted(queryParams);

        return query;
    }

    public virtual int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams)
    {
        var query = GetQueryFilteredSorted(gameId, queryParams);

        return query.Count();
    }

    public virtual List<T> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
    {
        var query = GetQueryFilteredSorted(gameId, queryParams);
        foreach (var kvp in queryParams)
        {
            query = kvp.Key switch
            {
                "limit" => query.Take(int.Parse(kvp.Value)),
                "skip" => query.Skip(int.Parse(kvp.Value)),
                _ => query
            };
        }


        return query.ToList();
    }
}