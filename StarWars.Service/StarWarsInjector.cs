using Microsoft.Extensions.DependencyInjection;
using StarWars.Model;
using StarWars.Repository;
using StarWars.Repository.Interfaces;
using StarWars.Service.Interfaces;

namespace StarWars.Service;

public static class StarWarsInjector
{
    public static void AddStarWarsServices(this IServiceCollection services)
    {
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IEmpireService, EmpireService>();
        services.AddScoped<IGameSoldierService, GameSoldierService>();
        services.AddScoped<IRebelService, RebelService>();
        services.AddScoped<IRoundService, RoundService>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IRoundRepository, RoundRepository>();
        services.AddScoped<ISoldierService, SoldierService>();
        services.AddScoped(typeof(IService<>), typeof(Service<>));
        services.AddScoped<IService<Game>, GameService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IRepository<Game>, GameRepository>();
        services.AddScoped<GameSoldierRepository>();
        services.AddScoped(typeof(GamePageRepository<>));

    }
}