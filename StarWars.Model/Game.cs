namespace StarWars.Model;

public class Game
{
    public int Id { get; set; }

    public List<GameSoldier> Soldiers { get; set; } = new();

    public List<Round> Rounds { get; set; } = new();

    public int MaxRound { get; set; }


}