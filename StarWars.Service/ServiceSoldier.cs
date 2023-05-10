using Microsoft.AspNetCore.JsonPatch;
using StarWars.Model;

namespace StarWars.Controllers;

public class ServiceSoldier : Service<Soldier>
{
    private readonly StarWarsDbContext _context;

    public ServiceSoldier(StarWarsDbContext context) : base(context)
    {
        _context = context;
    }

    public override Soldier Patch(int id, JsonPatchDocument<Soldier> patch)
    {
        var rndSrv = new ServiceRound(_context);

        var soldier = base.Patch(id, patch);

        rndSrv.PatchRoundsDamage(soldier);

        return soldier;
    }
}