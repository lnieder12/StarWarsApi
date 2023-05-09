using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using StarWars.Model;

namespace StarWars.Controllers;

public class RoundRepository : Repository<Round>
{
    public RoundRepository(StarWarsDbContext ctx) : base(ctx)
    {
    }

    public Round GetInclude(int id)
    {
        return ctx.Set<Round>().Include(r => r.Attacker)
            .Include(r => r.Defender)
            .FirstOrDefault(r => r.Id == id);
    }

    public List<Round> GetAll()
    {
        return All().ToList();
    }

    public IIncludableQueryable<Round, Soldier> All()
    {
        return ctx.Rounds.Include(r => r.Attacker).Include(r => r.Defender);
    }
 



    public List<Round> GetPage(int lastId, int nbRow)
    {
        return All()
            .OrderBy(rnd => rnd.Id)
            .Where(rnd => rnd.Id > lastId)
            
            .Take(nbRow)
            .ToList();
    }

}