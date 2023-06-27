using StarWars.Model;

namespace StarWars.Service;

public interface IEmpireService : IService<Empire>
{
    Empire CreateRandom(int number);
}