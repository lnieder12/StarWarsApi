using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StarWars.Model;

namespace StarWars.Controllers;

public class Repository<T> where T : class
{
    protected StarWarsDbContext ctx;

    public Repository(StarWarsDbContext ctx)
    {
        this.ctx = ctx;
    }

    public T Add(T obj)
    {
        ctx.Set<T>().Add(obj);
        ctx.SaveChanges();

        return obj;
    }

    public List<T> GetAll()
    {
        return ctx.Set<T>().ToList();
    }

    public virtual T Get(int id) 
    {
        return ctx.Set<T>().Find(id);
    }

    public T Patch(T obj, JsonPatchDocument<T> patch)
    {
        patch.ApplyTo(obj);
        ctx.SaveChanges();
        return obj;
    }

    public void Delete(T obj)
    {
        ctx.Set<T>().Remove(obj);
        ctx.SaveChanges();
    }

    public virtual List<T> Page(int skip, int pageSize)
    {
        return ctx.Set<T>()
            .Skip(skip)
            .Take(pageSize)
            .ToList();
    }

}