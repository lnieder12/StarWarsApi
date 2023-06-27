using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using StarWars.Model;
using StarWars.Repository;
using StarWars.Repository.Interfaces;

namespace StarWars.Service;

public class RoundService : GamePageService<Round>, IRoundService
{

    private readonly IRoundRepository _roundRepo;

    private readonly IService<Soldier> _sldSrv;


    public RoundService(IRepository<Round> repo, GamePageRepository<Round> pageRepo, IRoundRepository roundRepo, IService<Soldier> sldSrv) : base(repo, pageRepo)
    {
        _roundRepo = roundRepo;
        _sldSrv = sldSrv;
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

    public void PostAll(List<Round> rounds)
    {
        _roundRepo.PostAll(rounds);
    }

    public Round GetInclude(int id)
    {
        return _roundRepo.GetInclude(id);
    }

    public override List<Round> GetPage(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return _roundRepo.GetPage(gameId, queryParams);
    }

    public override int GetCountOnQuery(int gameId, Dictionary<string, StringValues> queryParams)
    {
        return _roundRepo.GetPageCount(gameId, queryParams);
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