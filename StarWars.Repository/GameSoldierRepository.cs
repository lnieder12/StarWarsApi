using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Controllers;

public class GameSoldierRepository : FilterPageRepository<GameSoldier>
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

    public override IQueryable<GameSoldier> GetQueryFilteredSorted(int gameId, Dictionary<string, StringValues> queryParams)
    {
        var gsDb = ctx.Set<GameSoldier>();
        var query = gsDb
            .Include(gs => gs.Soldier)
            .Where(gs => gs.GameId == gameId);
            
        query = query.AddFiltersSorted(queryParams);

        return query;
    }

    // public int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams)
    // {
    //     var query = GetQueryFilteredSorted(gameId, queryParams);
    //
    //     return query.Count();
    // }
    //
    // public List<GameSoldier> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
    // {
    //     var query = GetQueryFilteredSorted(gameId, queryParams);
    //     foreach (var kvp in queryParams)
    //     {
    //         query = kvp.Key switch
    //         {
    //             "limit" => query.Take(int.Parse(kvp.Value)),
    //             "skip" => query.Skip(int.Parse(kvp.Value)),
    //             _ => query
    //         };
    //     }
    //
    //
    //     return query.ToList();
    // }

}