using Microsoft.AspNetCore.JsonPatch;

namespace StarWars.Service;

public interface IService<T> where T : class
{
    public T Add(T obj);
    public List<T> GetAll();
    public T Get(int id);
    public T Patch(int id, JsonPatchDocument<T> patch);
    public bool Delete(int id);
}