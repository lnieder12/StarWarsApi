using Microsoft.AspNetCore.JsonPatch;
using StarWars.Model;

namespace StarWars.Service;

public interface IRebelService : IService<Rebel>
{
    Rebel CreateRandom(int number);
}