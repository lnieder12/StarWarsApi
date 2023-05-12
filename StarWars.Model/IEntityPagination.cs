namespace StarWars.Model;

public interface IEntityPagination
{

    public ICollection<string> GetInclude()
    {
        return GetType().GetProperties().Select(prop => prop.Name).ToList();
    }

}