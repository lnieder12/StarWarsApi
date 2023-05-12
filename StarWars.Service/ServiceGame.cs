using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class ServiceGame : Service<Game>
{
    private readonly ServiceEmpire _empSrv;

    private readonly GameRepository _gameRepo;

    private readonly ServiceGameSoldier _gsSrv;

    private readonly ServiceRebel _rebSrv;

    private readonly ServiceRound _rndSrv;

    private readonly ServiceSoldier _sldSrv;

    public ServiceGame(StarWarsDbContext context) : base(context)
    {
        _gameRepo = new GameRepository(context);
        _sldSrv = new ServiceSoldier(context);
        _rndSrv = new ServiceRound(context);
        _rebSrv = new ServiceRebel(context);
        _empSrv = new ServiceEmpire(context);
        _gsSrv = new ServiceGameSoldier(context);
    }

    public Game CreateGame(int rebels, int empires, int nbRounds)
    {
        Game game;
        if (nbRounds > 0)
            game = Add(new Game
            {
                MaxRound = nbRounds
            });
        else
            game = Add(new Game());


        for (var i = 1; i <= rebels; i++) AddSoldier(game.Id, _rebSrv.CreateRandom(i).Id);
        for (var y = 1; y <= empires; y++) AddSoldier(game.Id, _empSrv.CreateRandom(y).Id);

        return game;
    }

    public Game CreateSelectedGame(RebelsEmpires soldiers, int nbRounds)
    {
        Game game;
        if (nbRounds > 0)
            game = Add(new Game
            {
                MaxRound = nbRounds
            });
        else
            game = Add(new Game());

        soldiers.Rebels.ForEach(reb => AddSoldier(game.Id, reb.Id));
        soldiers.Empires.ForEach(emp => AddSoldier(game.Id, emp.Id));

        return game;
    }

    public override List<Game> GetAll()
    {
        return _gameRepo.GetAll();
    }

    public Game GetIncludeSoldiers(int id)
    {
        return _gameRepo.GetIncludeSoldiers(id);
    }

    public Game GetIncludeRounds(int id)
    {
        return _gameRepo.GetIncludeRounds(id);
    }

    public Soldier AddSoldier(int id, int soldierId)
    {
        if (SoldierInGame(id, soldierId))
            return null;

        var soldier = _sldSrv.Get(soldierId);

        var game = GetIncludeSoldiers(id);

        var gs = new GameSoldier
        {
            GameId = game.Id,
            Game = game,
            SoldierId = soldierId,
            Soldier = soldier,
            Health = soldier.MaxHealth
        };

        _gsSrv.Add(gs);

        game.Soldiers.Add(gs);

        return soldier;
    }

    public Soldier PatchSoldier(int id, int soldierId, JsonPatchDocument<Soldier> patch)
    {
        if (!SoldierInGame(id, soldierId))
            return null;

        var soldier = _sldSrv.Get(soldierId);

        var nSoldier = _sldSrv.Patch(soldier.Id, patch);

        return nSoldier;
    }

    public Round AddRound(int id, int roundId)
    {
        var round = _rndSrv.Get(roundId);

        var game = GetIncludeRounds(id);

        if (game == null)
            return null;

        game.Rounds.Add(round);

        return round;
    }


    public List<Round> GetRounds(int id)
    {
        return GetIncludeRounds(id).Rounds;
    }

    public int NbRounds(int id)
    {
        return GetRounds(id).Count;
    }

    public bool SoldierInGame(int id, int soldierId)
    {
        var game = GetIncludeSoldiers(id);
        return game.Soldiers?.Any(gs => gs.SoldierId == soldierId) ?? false;
    }

    public List<GameSoldier> GetGs(int id)
    {
        return GetIncludeSoldiers(id).Soldiers;
    }

    public List<GameSoldier> GetGs<T>(int id) where T : Soldier
    {
        return GetIncludeSoldiers(id).Soldiers
            .Where(gs => gs.Soldier.GetType() == typeof(T) || gs.Soldier.GetType().IsSubclassOf(typeof(T))).ToList();
    }

    public List<Soldier> GetSoldiers(int id)
    {
        return GsToSoldier(GetGs(id));
    }

    public List<Soldier> GsToSoldier(List<GameSoldier> gameSoldiers)
    {
        var soldiers = new List<Soldier>();
        gameSoldiers.ForEach(gs => soldiers.Add(gs.Soldier));
        return soldiers;
    }

    public List<Rebel> GetRebels(int id)
    {
        return GsToSoldier(GetGs<Rebel>(id)).Cast<Rebel>().ToList();
    }

    public List<Empire> GetEmpires(int id)
    {
        return GsToSoldier(GetGs<Empire>(id)).Cast<Empire>().ToList();
    }

    private List<GameSoldier> FilterValidSoldiers(List<GameSoldier> soldiers)
    {
        return soldiers.FindAll(gs => gs.Health > 0);
    }

    public T GetRandom<T>(int id) where T : Soldier
    {
        var soldiers = GetValid<T>(id);
        if (soldiers.IsNullOrEmpty())
            return null;
        var random = new Random();
        return (T)soldiers[random.Next(soldiers.Count)].Soldier;
    }

    private List<GameSoldier> GetValid<T>(int id) where T : Soldier
    {
        return FilterValidSoldiers(GetGs<T>(id));
    }

    public int NbValidSoldier<T>(int id) where T : Soldier
    {
        return GetValid<T>(id).Count;
    }

    public Round Fight(int id)
    {
        var game = Get(id);

        var att = GetRandom<Soldier>(id);

        if (game == null || att == null || !SoldierInGame(id, att.Id)) return null;

        Soldier defender;

        if (att.GetType() == typeof(Empire))
            defender = GetRandom<Rebel>(id);
        else
            defender = GetRandom<Empire>(id);

        if (defender == null)
            return null;
        var defGs = _gsSrv.Get(id, defender.Id);
        var attGs = _gsSrv.Get(id, att.Id);
        attGs.Damage += att.Attack;
        defGs.Health -= att.Attack;

        if (defGs.Health < 0)
            defGs.Health = 0;

        var round = _rndSrv.AddRound(att.Id, defender.Id, game);
        round.HpLeft = defGs.Health;
        round.IsDead = defGs.Health <= 0;

        return AddRound(id, round.Id);
    }

    private SoldierScore GetSoldierScore(GameSoldier gs)
    {
        var score = new SoldierScore
        {
            GsId = gs.Id,
            Soldier = gs.Soldier,
            Score = gs.Score
        };
        return score;
    }


    public List<SoldierScore> GetSoldierScoresPage(int id, Dictionary<string, StringValues> queryParams)
    {
        var scores = new List<SoldierScore>();
        _gsSrv.GetPage(id, queryParams).ForEach(gs => scores.Add(GetSoldierScore(gs)));

        return scores;
    }

    public List<Round> GetRoundsPage(int id, Dictionary<string, StringValues> queryParams)
    {
        var rounds = _rndSrv.GetPage(id, queryParams);

        return rounds;
    }

    public int GetScoresFilteredCount(int id, Dictionary<string, StringValues> queryParams)
    {
        return _gsSrv.GetCountOnQuery(id, queryParams);
    }


    public int GetRoundsFilteredCount(int id, Dictionary<string, StringValues> queryParams)
    {
        return _rndSrv.GetCountOnQuery(id, queryParams);
    }


    public int TeamScore<T>(int id) where T : Soldier
    {
        var score = 0;
        GetGs<T>(id).ForEach(sld => score += sld.Score);
        return score;
    }

    public List<Round> MultipleFights(int id, int nb)
    {
        var rounds = new List<Round>();

        Round round;

        if (nb > 0)
        {
            var i = 0;

            do
            {
                round = Fight(id);
                if (round != null)
                    rounds.Add(round);
                i++;
            } while (round != null && i < nb);
        }
        else
        {
            do
            {
                round = Fight(id);
                if (round != null)
                    rounds.Add(round);
            } while (round != null);
        }

        return rounds;
    }

    public bool EnoughSoldiers(int id)
    {
        return NbValidSoldier<Rebel>(id) > 0 && NbValidSoldier<Empire>(id) > 0;
    }

    public string WinnerTeam(int id)
    {
        string winner;
        if (!EnoughSoldiers(id))
        {
            winner = GetValid<Empire>(id).Count > 0 ? "Empires won" : "Rebels won";
        }
        else
        {
            var empScore = TeamScore<Empire>(id);
            var rebScore = TeamScore<Rebel>(id);
            if (empScore > rebScore)
                winner = "Empires won with " + empScore + " points";
            else
                winner = "Rebels won with " + rebScore + " points";
        }

        return winner;
    }
}