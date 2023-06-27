using Microsoft.AspNetCore.JsonPatch;
using StarWars.Model;

namespace StarWars.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected StarWarsDbContext Ctx;

    public Repository(StarWarsDbContext ctx)
    {
        Ctx = ctx;
    }

    public T Add(T obj)
    {
        Ctx.Set<T>().Add(obj);
        Ctx.SaveChanges();

        return obj;
    }

    public List<T> GetAll()
    {
        return Ctx.Set<T>().ToList();
    }

    public virtual T Get(int id) 
    {
        return Ctx.Set<T>().Find(id);
    }

    public T Patch(T obj, JsonPatchDocument<T> patch)
    {
        patch.ApplyTo(obj);
        Ctx.SaveChanges();
        return obj;
    }

    public void Delete(T obj)
    {
        Ctx.Set<T>().Remove(obj);
        Ctx.SaveChanges();
    }

}