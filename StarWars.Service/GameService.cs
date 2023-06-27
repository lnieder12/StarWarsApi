using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using StarWars.Model;
using StarWars.Repository;
using StarWars.Service.Interfaces;

namespace StarWars.Service;

public class GameService : Service<Game>, IGameService
{
    private readonly IEmpireService _empSrv;

    private readonly IGameSoldierService _gsSrv;

    private readonly IGameRepository _gameRepo;

    private readonly IRebelService _rebSrv;

    private readonly IRoundService _rndSrv;

    private readonly ISoldierService _sldSrv;


    public GameService(IRepository<Game> repo, IEmpireService empSrv, IGameSoldierService gsSrv, IGameRepository gameRepo, IRebelService rebSrv, IRoundService rndSrv, ISoldierService sldSrv) : base(repo)
    {
        _empSrv = empSrv;
        _gsSrv = gsSrv;
        _gameRepo = gameRepo;
        _rebSrv = rebSrv;
        _rndSrv = rndSrv;
        _sldSrv = sldSrv;
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

    public Game GetIncludeAll(int id)
    {
        return _gameRepo.GetIncludeAll(id);
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

    public void AddRound(Game game, Round round)
    {
        game.Rounds.Add(round);
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

    public List<GameSoldier> GetGs<T>(Game game) where T : Soldier
    {
        return game.Soldiers
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
        return GsToSoldier(
            GetGs<Rebel>(GetIncludeSoldiers(id)))
            .Cast<Rebel>().ToList();
    }

    public List<Empire> GetEmpires(int id)
    {
        return GsToSoldier(
            GetGs<Empire>(GetIncludeSoldiers(id)))
            .Cast<Empire>().ToList();
    }

    public List<GameSoldier> FilterValidSoldiers(List<GameSoldier> soldiers)
    {
        return soldiers.FindAll(gs => gs.Health > 0);
    }

    public GameSoldier GetRandom<T>(Game game) where T : Soldier
    {
        List<GameSoldier> soldiers = 
            GetValid<T>(game.Soldiers.IsNullOrEmpty() ? GetIncludeSoldiers(game.Id) : game);
        if (soldiers.IsNullOrEmpty())
            return null;
        var random = new Random();
        return soldiers[random.Next(soldiers.Count)];
    }

    public List<GameSoldier> GetValid<T>(Game game) where T : Soldier
    {
        return FilterValidSoldiers(GetGs<T>(game));
    }

    public int NbValidSoldier<T>(Game game) where T : Soldier
    {
        return GetValid<T>(game).Count;
    }

    public int GetNbValidSoldier<T>(int id) where T : Soldier
    {
        return NbValidSoldier<T>(GetIncludeSoldiers(id));
    }

    public Round DoFight(Game game)
    {
        var att = GetRandom<Soldier>(game);

        var def= att.GetType() == typeof(Empire) ? 
            GetRandom<Rebel>(game) : GetRandom<Empire>(game);

        if (att == null || def == null)
            return null;

        att.Damage += att.Soldier.Attack;
        def.Health -= att.Soldier.Attack;
        if (def.Health < 0)
            def.Health = 0;

        var round = new Round()
        {
            Attacker = att.Soldier,
            Defender = def.Soldier,
            Damage = att.Soldier.Attack,
            Game = game,
            GameId = game.Id,
            HpLeft = def.Health,
        };

        AddRound(game, round);

        return round;

    }

    public Round Fight(int id)
    {
        var game = GetIncludeAll(id);
        var round = DoFight(game);
        _gameRepo.SaveGame(game);
        return round;
    }

    public SoldierScore GetSoldierScore(GameSoldier gs)
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
        GetGs<T>(GetIncludeSoldiers(id)).ForEach(sld => score += sld.Score);
        return score;
    }

    public List<Round> MultipleFights(int id, int nb)
    {
        var rounds = new List<Round>();

        Round round;

        var game = GetIncludeAll(id);

        if (nb > 0)
        {
            var i = 0;

            do
            {
                round = DoFight(game);
                if (round != null)
                    rounds.Add(round);
                i++;
            } while (round != null && i < nb);
        }
        else
        {
            do
            {
                round = DoFight(game);
                if (round != null)
                    rounds.Add(round);
            } while (round != null);
        }

        _rndSrv.PostAll(rounds);

        _gameRepo.SaveGame(game);

        return rounds;
    }

    public bool EnoughSoldiers(int id)
    {
        return NbValidSoldier<Rebel>(GetIncludeSoldiers(id)) > 0 
               && NbValidSoldier<Empire>(GetIncludeSoldiers(id)) > 0;
    }

    public string WinnerTeam(int id)
    {
        string winner;
        if (!EnoughSoldiers(id))
        {
            winner = GetValid<Empire>(GetIncludeSoldiers(id)).Count > 0 ? "Empires won" : "Rebels won";
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