using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using StarWars.Model;

namespace StarWars.Controllers;

public class ServiceGame : Service<Game>
{
    private readonly StarWarsDbContext _context;

    private GameRepository gameRepo;

    private ServiceGameSoldier gsSrv;

    public ServiceGame(StarWarsDbContext context) : base(context)
    {
        _context = context;
        gameRepo = new GameRepository(context);
        gsSrv = new ServiceGameSoldier(context);
    }

    public Game CreateGame(int rebels, int empires, int nbRounds)
    {
        var srvRebel = new ServiceRebel(_context);

        var srvEmpire = new ServiceEmpire(_context);

        if (srvEmpire == null || srvRebel == null) throw new ArgumentNullException(nameof(srvEmpire));

        Game game;
        if (nbRounds > 0)
        {
            game = this.Add(new Game()
            {
                MaxRound = nbRounds,
            });
        }
        else
            game = this.Add(new Game());


        for (var i = 1; i <= rebels; i++)
        {
            this.AddSoldier(game.Id, srvRebel.CreateRandom(i).Id);
        }
        for (var y = 1; y <= empires; y++)
        {
            this.AddSoldier(game.Id, srvEmpire.CreateRandom(y).Id);
        }

        return game;
    }

    public Game CreateSelectedGame(List<Rebel> rebels, List<Empire> empires, int nbRounds)
    {
        Game game;
        if (nbRounds > 0)
        {
            game = this.Add(new Game()
            {
                MaxRound = nbRounds,
            });
        }
        else
            game = this.Add(new Game());

        rebels.ForEach(reb => this.AddSoldier(game.Id, reb.Id));
        empires.ForEach(emp => this.AddSoldier(game.Id, emp.Id));

        return game;
    }

    public override List<Game> GetAll()
    {
        return gameRepo.GetAll();
    }

    public Game GetIncludeSoldiers(int id)
    {
        return gameRepo.GetIncludeSoldiers(id);
    }
    public Game GetIncludeRounds(int id)
    {
        return gameRepo.GetIncludeRounds(id);
    }

    public Soldier GetSoldier(int id)
    {
        var srv = new Service<Soldier>(_context);
        
        return srv.Get(id);
    }

    public Soldier AddSoldier(int id, int soldierId)
    {
        if (SoldierInGame(id, soldierId))
            return null;

        var soldier = GetSoldier(soldierId);

        var game = GetIncludeSoldiers(id);

        if (game == null)
        {
            return null;
        }

        

        var gs = new GameSoldier()
        {
            GameId = game.Id,
            Game = game,
            SoldierId = soldierId,
            Soldier = soldier,
            Health = soldier.MaxHealth
        };

        gsSrv.Add(gs);

        game.Soldiers.Add(gs);

        _context.SaveChanges();

        return soldier;

    }

    public Soldier PatchSoldier(int id, int soldierId, JsonPatchDocument<Soldier> patch)
    {
        if (!SoldierInGame(id, soldierId))
            return null;

        var srv = new Service<Soldier>(_context);

        var soldier = srv.Get(soldierId);

        var nSoldier = srv.Patch(soldier.Id, patch);

        return nSoldier;
    }

    public Round AddRound(int id, int roundId)
    {
        var srv = new ServiceRound(_context);

        var round = srv.Get(roundId);

        var game = GetIncludeRounds(id);

        if (game == null)
            return null;

        game.Rounds.Add(round);

        _context.SaveChanges();

        return round;
    }


    public List<Round> GetRounds(int id)
    {
        return GetIncludeRounds(id)?.Rounds;
    }

    public int NbRounds(int id)
    {
        return GetRounds(id).Count;
    }

    public bool SoldierInGame(int id, int soldierId)
    {
        var game = GetIncludeSoldiers(id);
        return game?.Soldiers?.Any(gs => gs.SoldierId == soldierId) ?? false;
    }

    public List<GameSoldier> GetGS(int id)
    {
        return GetIncludeSoldiers(id)?.Soldiers;
    }

    public List<GameSoldier> GetGSRebel(int id)
    {
        return GetIncludeSoldiers(id)?.Soldiers.Where(gs => gs.Soldier.SoldierType == "Rebel").ToList();
    }

    public List<GameSoldier> GetGSEmpire(int id)
    {
        return GetIncludeSoldiers(id)?.Soldiers.Where(gs => gs.Soldier.SoldierType == "Empire").ToList();
    }

    public List<Soldier> GetSoldiers(int id)
    {
        return gsToSoldier(GetGS(id));
    }

    public List<Soldier> gsToSoldier(List<GameSoldier> gameSoldiers)
    {
        var soldiers = new List<Soldier>();
        gameSoldiers.ForEach(gs => soldiers.Add(gs.Soldier));
        return soldiers;
    }

    public List<Rebel> GetRebels(int id)
    {
        return gsToSoldier(GetGSRebel(id)).Cast<Rebel>().ToList();
    }

    public List<Empire> GetEmpires(int id)
    {
        return gsToSoldier(GetGSEmpire(id)).Cast<Empire>().ToList();
    }

    public List<GameSoldier> FilterValideSoldiers(List<GameSoldier> soldiers, int gameId)
    {
        return soldiers.FindAll(gs => gs.Health > 0);
    }

    public Soldier GetRandomSoldier(int id)
    {
        var soldiers = FilterValideSoldiers(GetGS(id), id);
        if (soldiers.IsNullOrEmpty())
            return null;
        Random random = new Random();

        return soldiers[random.Next(soldiers.Count())].Soldier;
    }

    public Rebel GetRandomRebel(int id)
    {
        var rebels = GetValideRebels(id);
        if (rebels.IsNullOrEmpty())
            return null;
        Random random = new Random();
        return (Rebel)rebels[random.Next(rebels.Count())].Soldier;
    }

    public Empire GetRandomEmpire(int id)
    {
        var empires = GetValideEmpires(id);
        if (empires.IsNullOrEmpty())
            return null;
        Random random = new Random();
        return (Empire)empires[random.Next(empires.Count())].Soldier;
    }

    public List<GameSoldier> GetValideRebels(int id)
    {
        return FilterValideSoldiers(GetGSRebel(id), id);
    }

    public List<GameSoldier> GetValideEmpires(int id)
    {
        return FilterValideSoldiers(GetGSEmpire(id), id);
    }

    public int NbValideRebels(int id)
    {
        return GetValideRebels(id).Count;
    }

    public int NbValideEmpires(int id)
    {
        return GetValideEmpires(id).Count;
    }

    public Round Fight(int id)
    {
        var game = Get(id);

        var att = GetRandomSoldier(id);

        if(game == null || att == null || !SoldierInGame(id, att.Id))
        {
            return null;
        }

        var srv = new ServiceRound(_context);

        Soldier defender;

        if (att.GetType() == typeof(Empire))
        {
            defender = GetRandomRebel(id);
        }
        else
        {
            defender = GetRandomEmpire(id);
        }

        if (defender == null)
            return null;
        var defGs = gsSrv.Get(id, defender.Id);
        var attGs = gsSrv.Get(id, att.Id);
        attGs.Damage += att.Attack;
        defGs.Health -= att.Attack;

        if (defGs.Health < 0)
            defGs.Health = 0;

        var round = srv.AddRound(att.Id, defender.Id, game);
        round.HpLeft = defGs.Health;
        round.IsDead = defGs.Health <= 0;

        return AddRound(id, round.Id);

    }

    public List<Round> GetSoldierRoundsAsAttacker(int id, int soldierId)
    {
        if (!SoldierInGame(id, soldierId))
            return null;
        return GetRounds(id).FindAll(r => r.Attacker.Id == soldierId);
    }

    public SoldierScore GetSoldierScore(int id, GameSoldier gs)
    {
        var score = new SoldierScore
        {
            GsId = gs.Id,
            Soldier = gs.Soldier,
            Score = gs.Score,
        };
        return score;
    }

    public List<SoldierScore> GetSoldierScores(int id)
    {
        var scores = new List<SoldierScore>();
        GetGS(id).ForEach(gs => scores.Add(this.GetSoldierScore(id, gs)));
        return scores;
    }

    public List<SoldierScore> GetSoldierScoresPage(int id, Dictionary<string, StringValues> queryParams)
    {
        var gsRepo = new GameSoldierRepository(_context);
        var scores = new List<SoldierScore>();
        gsRepo.GetPage(id, queryParams).ForEach(gs => scores.Add(GetSoldierScore(id, gs)));

        return scores;
    }

    public List<Round> GetRoundsPage(int id, Dictionary<string, StringValues> queryParams)
    {
        var rndRepo = new RoundRepository(_context);
        var rounds = rndRepo.GetPage(id, queryParams);

        return rounds;
    }

    public int GetScoreFilteredCount(int id, Dictionary<string, StringValues> queryParams)
    {
        var gsRepo = new GameSoldierRepository(_context);
        return gsRepo.GetCountOnQuery(id, queryParams);
    }


    public int GetRoundFilteredCount(int id, Dictionary<string, StringValues> queryParams)
    {
        var rndRepo = new RoundRepository(_context);
        return rndRepo.GetCountOnQuery(id, queryParams);
    }

    public int RebelScore(int id)
    {
        var score = 0;
        GetGSRebel(id).ForEach(rebel => score += rebel.Score);
        return score;
    }

    public int EmpireScore(int id)
    {
        var score = 0;
        GetGSEmpire(id).ForEach(empire => score += empire.Score);
        return score;
    }

    public List<Round> MultipleFights(int id, int nb)
    {

        List<Round> rounds = new List<Round>();

        Round round;

        if (nb > 0)
        {
            int i = 0;

            do
            {
                round = Fight(id);
                if(round != null)
                    rounds.Add(round);
                i++;
            }
            while (round != null && i < nb);
        }
        else
        {
            do
            {
                round = Fight(id);
                if (round != null)
                    rounds.Add(round);
            }       
            while (round != null) ;
        }
        return rounds;
    }

    public bool EnoughSoldiers(int id)
    {
        return NbValideRebels(id) > 0 && NbValideEmpires(id) > 0;
    }

    public string WinnerTeam(int id)
    {
        var winner = "";
        if (!EnoughSoldiers(id))
        {
            if (GetValideEmpires(id).Count > 0)
            {
                winner = "Empires won";
            }
            else
                winner = "Rebels won";
        }
        else
        {
            var empScore = EmpireScore(id);
            var rebScore = RebelScore(id);
            if (empScore > rebScore)
                winner = "Empires won with " + empScore + " points";
            else
                winner = "Rebels won with " + rebScore + " points";
        }
        return winner;
    }

}