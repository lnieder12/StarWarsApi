using Microsoft.AspNetCore.JsonPatch;
using StarWars.Model;

namespace StarWars.Service;

public class ServiceSoldier : Service<Soldier>
{

    private readonly ServiceRound _rndSrv;

    public ServiceSoldier(StarWarsDbContext context) : base(context)
    {
        _rndSrv = new ServiceRound(context);
    }

    public override Soldier Patch(int id, JsonPatchDocument<Soldier> patch)
    {
        var soldier = base.Patch(id, patch);

        _rndSrv.PatchRoundsDamage(soldier);

        return soldier;
    }
}