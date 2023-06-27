using Microsoft.AspNetCore.JsonPatch;
using StarWars.Model;
using StarWars.Repository;

namespace StarWars.Service;

public class Service<T> : IService<T> where T : class
{
    public Repository<T> Repo;

    public Service(StarWarsDbContext context)
    {
        Repo = new Repository<T>(context);
    }

    public virtual T Add(T obj)
    {
        return Repo.Add(obj);
    }

    public virtual List<T> GetAll()
    {
        return Repo.GetAll();
    }

    public virtual T Get(int id)
    {
        return Repo.Get(id);
    }

    public virtual T Patch(int id, JsonPatchDocument<T> patch)
    {
        return Repo.Patch(Get(id), patch);
    }

    public bool Delete(int id)
    {
        var item = Repo.Get(id);
        if(item == null)
            return false;

        Repo.Delete(item);

        return true;
    }

}