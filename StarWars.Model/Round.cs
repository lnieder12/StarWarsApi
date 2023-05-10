using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace StarWars.Model;

[Serializable()]
public class Round
{
    public int Id { get; set; }

    [AllowNull]
    public Soldier Attacker { get; set; }

    [AllowNull]
    public Soldier Defender { get; set; }
    public int Damage { get; set; }

    public bool IsDead { get; set; }

    public int HpLeft { get; set; }

    public Game Game { get; set; }

    public int GameId { get; set; }
}