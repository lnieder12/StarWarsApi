using Microsoft.AspNetCore.JsonPatch;
using StarWars.Model;
using StarWars.Repository;
using StarWars.Service.Interfaces;

namespace StarWars.Service;

public class SoldierService : Service<Soldier>, ISoldierService
{

    private readonly IRoundService _rndSrv;


    public SoldierService(IRepository<Soldier> repo, IRoundService rndSrv) : base(repo)
    {
        _rndSrv = rndSrv;
    }

    public override Soldier Patch(int id, JsonPatchDocument<Soldier> patch)
    {
        var soldier = base.Patch(id, patch);

        _rndSrv.PatchRoundsDamage(soldier);

        return soldier;
    }
}