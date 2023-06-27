using Microsoft.AspNetCore.JsonPatch;
using StarWars.Model;

namespace StarWars.Service;


public class SoldierService : Service<Soldier>
{

    private readonly RoundService _rndSrv;

    public SoldierService(StarWarsDbContext context) : base(context)
    {
        _rndSrv = new RoundService(context);
    }

    public override Soldier Patch(int id, JsonPatchDocument<Soldier> patch)
    {
        var soldier = base.Patch(id, patch);

        _rndSrv.PatchRoundsDamage(soldier);

        return soldier;
    }
}