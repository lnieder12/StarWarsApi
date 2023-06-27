using Microsoft.AspNetCore.JsonPatch;

namespace StarWars.Repository;

public interface IRepository<T> where T : class
{
    T Add(T obj);
    List<T> GetAll();
    T Get(int id);
    T Patch(T obj, JsonPatchDocument<T> patch);
    void Delete(T obj);
}