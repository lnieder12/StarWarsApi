using Microsoft.AspNetCore.JsonPatch;

namespace StarWars.Repository;

public interface IRepository<T> where T : class
{
    public T Add(T obj);
    public List<T> GetAll();
    public T Get(int id);
    public T Patch(T obj, JsonPatchDocument<T> patch);
    public void Delete(T obj);
}