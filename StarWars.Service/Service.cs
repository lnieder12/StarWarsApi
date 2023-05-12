using Microsoft.AspNetCore.JsonPatch;
using StarWars.Controllers;
using StarWars.Model;

namespace StarWars.Service;

public class Service<T> where T : class
{
    public Repository<T> Repo;

    public Service(StarWarsDbContext context)
    {
        this.Repo = new Repository<T>(context);
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

    public virtual List<T> GetPage(int skip, int pageSize)
    {
        return Repo.Page(skip, pageSize);
    }


}