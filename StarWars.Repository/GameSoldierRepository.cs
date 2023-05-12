using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Repository;

public class GameSoldierRepository : FilterPageRepository<GameSoldier>
{
    public GameSoldierRepository(StarWarsDbContext ctx) : base(ctx)
    {
    }

    public GameSoldier Get(int gameId, int soldierId)
    {
        return Ctx.Set<GameSoldier>()
            .FirstOrDefault(gs => gs.GameId == gameId && gs.SoldierId == soldierId);
    }

    public override IQueryable<GameSoldier> GetQueryFilteredSorted(int gameId, Dictionary<string, StringValues> queryParams)
    {
        var gsDb = Ctx.Set<GameSoldier>();
        var query = gsDb
            .Include(gs => gs.Soldier)
            .Where(gs => gs.GameId == gameId);
            
        query = query.AddFiltersSorted(queryParams);

        return query;
    }

}