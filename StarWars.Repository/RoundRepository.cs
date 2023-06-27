using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Primitives;
using StarWars.Model;
using StarWars.Repository.Interfaces;

namespace StarWars.Repository;

public class RoundRepository : GamePageRepository<Round>, IRoundRepository
{
    public RoundRepository(StarWarsDbContext ctx) : base(ctx)
    {
    }

    public Round GetInclude(int id)
    {
        return All()
            .FirstOrDefault(r => r.Id == id);
    }

    public List<Round> GetAll()
    {
        return All().ToList();
    }

    public void PostAll(List<Round> rounds)
    {
        Ctx.Set<Round>().AddRange(rounds);
        Ctx.SaveChanges();
    }

    public IIncludableQueryable<Round, Soldier> All()
    {
        return Ctx.Rounds.Include(r => r.Attacker)
            .Include(r => r.Defender);
    }
 
    public List<Round> GetPage(int lastId, int nbRow)
    {
        return All()
            .OrderBy(rnd => rnd.Id)
            .Where(rnd => rnd.Id > lastId)
            
            .Take(nbRow)
            .ToList();
    }

    public override IQueryable<Round> GetQueryFilteredSorted(int gameId, Dictionary<string, StringValues> queryParams)
    {

        var query = All()
            .Where(rnd => rnd.GameId == gameId)
            ;

        query = query.AddFiltersSorted(queryParams);

        return query;
    }

}