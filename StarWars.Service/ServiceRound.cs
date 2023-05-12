using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class ServiceRound : GamePageService<Round>
{

    private readonly RoundRepository _roundRepo;


    private readonly Service<Soldier> _sldSrv;

    public ServiceRound(StarWarsDbContext context) : base(context)
    {
        _roundRepo = new RoundRepository(context);
        _sldSrv = new Service<Soldier>(context);
    }

    public override List<Round> GetAll()
    {
        return _roundRepo.GetAll();
    }

    public Round AddRound(int idAtt, int idDef, Game game)
    {
        var soldAtt = _sldSrv.Get(idAtt);

        var soldDef = _sldSrv.Get(idDef);

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
        return _roundRepo.GetInclude(id);
    }

    public List<Round> GetPage(int lastId, int pageSize)
    {
        return _roundRepo.GetPage(lastId, pageSize);
    }

    public List<Round> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return _roundRepo.GetPage(gameId, queryParams);
    }

    

    public void PatchRoundsDamage(Soldier attacker)
    {
        foreach (var rnd in _roundRepo.All())
        {
            var patch = new JsonPatchDocument<Round>();
            patch.Replace(round => round.Damage, attacker.Attack);
            Patch(rnd.Id, patch);
        }
    }

}