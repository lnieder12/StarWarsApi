using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Controllers;

public class GameSoldierRepository : Repository<GameSoldier>
{
    public GameSoldierRepository(StarWarsDbContext ctx) : base(ctx)
    {
    }

    public GameSoldier Get(int gameId, int soldierId)
    {
        return ctx.Set<GameSoldier>()
            .FirstOrDefault(gs => gs.GameId == gameId && gs.SoldierId == soldierId);
    }

    public GameSoldier GetInclude(int gameId, int soldierId)
    {
        return ctx.Set<GameSoldier>().Include(gs => gs.Game)
            .Include(gs => gs.Soldier)
            .FirstOrDefault(gs => gs.GameId == gameId && gs.SoldierId == soldierId);
    }

    public IQueryable<GameSoldier> GetQueryFilteredSorted(int gameId, Dictionary<string, StringValues> queryParams)
    {
        var gsDb = ctx.Set<GameSoldier>();
        var query = gsDb
            .Include(gs => gs.Soldier)
            .Where(gs => gs.GameId == gameId);

        foreach (var kvp in queryParams)
        {
            if (kvp.Key == "sort")
            {
                var type = kvp.Value.ToString().Split(':');
                if (type[1] == "desc")
                {
                    query = query.OderByDynamic(type[0]);
                }
                else
                {
                    query = query.OderByDynamic(type[0], false);
                }
            }
            else if (kvp.Key != "limit" && kvp.Key != "skip")
            {
                query = query.FilterDynamic(kvp.Key, kvp.Value.ToString());
            }
        }

        return query;
    }

    public int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams)
    {
        var query = GetQueryFilteredSorted(gameId, queryParams);

        return query.Count();
    }

    public List<GameSoldier> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
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