using Microsoft.EntityFrameworkCore;
using StarWars.Model;

namespace StarWars.Repository;

public class GameRepository : Repository<Game>, IGameRepository
{
    public GameRepository(StarWarsDbContext ctx) : base(ctx)
    {
    }
    public Game GetIncludeRounds(int id)
    {
        return Ctx.Set<Game>()
            .Include(g => g.Rounds).ThenInclude(r => r.Defender)
            .Include(g => g.Rounds).ThenInclude(r => r.Attacker)
            .FirstOrDefault(g => g.Id == id);
    }

    public void SaveGame(Game game)
    {
        Ctx.Set<Game>().Update(game);
        Ctx.SaveChanges();
    }

    public Game GetIncludeSoldiers(int id)
    {
        return Ctx.Set<Game>().Include(g => g.Soldiers).ThenInclude(gs => gs.Soldier).FirstOrDefault(g => g.Id == id);
    }

    public Game GetIncludeAll(int id)
    {
        return Ctx.Set<Game>()
            .Include(g => g.Rounds).ThenInclude(r => r.Defender)
            .Include(g => g.Rounds).ThenInclude(r => r.Attacker)
            .Include(g => g.Soldiers).ThenInclude(gs => gs.Soldier)
            .FirstOrDefault(g => g.Id == id);
    }

    public new List<Game> GetAll()
    {
        return Ctx.Set<Game>().Include(g => g.Soldiers).ToList();
    }

    public override Game Get(int id)
    {
        return Ctx.Set<Game>()
            .Include(g => g.Soldiers)
            .ThenInclude(gs => gs.Soldier)
            .FirstOrDefault(g => g.Id == id)!;
    }
}