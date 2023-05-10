using System.Diagnostics;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using StarWars.Model;

namespace StarWars.Controllers;

public class ServiceRound : Service<Round>
{
    private StarWarsDbContext context;

    private RoundRepository roundRepo;

    private ServiceGameSoldier gsSrv;

    public ServiceRound(StarWarsDbContext context) : base(context)
    {
        this.context = context;
        this.roundRepo = new RoundRepository(context);
        gsSrv = new ServiceGameSoldier(context);
    }

    public override List<Round> GetAll()
    {
        return roundRepo.GetAll();
    }

    public Round AddRound(int idAtt, int idDef, Game game)
    {
        var svSoldier = new Service<Soldier>(context);

        var soldAtt = svSoldier.Get(idAtt);

        var soldDef = svSoldier.Get(idDef);

        if(soldAtt == null || soldDef == null || soldAtt.GetType() == soldDef.GetType())
        {
            return null;
        }

        var round = new Round
        {
            Attacker = soldAtt,
            Defender = soldDef,
            Damage = soldAtt.Attack,
            Game = game,
            GameId = game.Id,
        };

        

        Add(round);


        return round;
    }

    public Round GetInclude(int id)
    {
        return roundRepo.GetInclude(id);
    }

    public List<Round> GetPage(int lastId, int pageSize)
    {
        return roundRepo.GetPage(lastId, pageSize);
    }

    public void PatchRoundsDamage(Soldier attacker)
    {
        foreach (var rnd in roundRepo.All())
        {
            var patch = new JsonPatchDocument<Round>();
            patch.Replace(rnd => rnd.Damage, attacker.Attack);
            Patch(rnd.Id, patch);
        }
    }

}
