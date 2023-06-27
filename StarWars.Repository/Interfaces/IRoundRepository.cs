using Microsoft.EntityFrameworkCore.Query;
using StarWars.Model;

namespace StarWars.Repository.Interfaces;

public interface IRoundRepository : IGamePageRepository<Round>
{
    Round GetInclude(int id);
    void PostAll(List<Round> rounds);
    IIncludableQueryable<Round, Soldier> All();
    List<Round> GetPage(int lastId, int nbRow);
}