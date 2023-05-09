using System.Linq.Expressions;

namespace StarWars.Controllers;

public static class QueryableExtensions
{
    public static IQueryable<T> FilterDynamic<T>(this IQueryable<T> query, string fieldName, string values)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var prop = GetProperty(param, fieldName);
        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var body = Expression.Call(prop, method,
            Expression.Constant(values, typeof(string)));
        var predicate = Expression.Lambda<Func<T, bool>>(body, param);

        return query.Where(predicate);
    }

    private static MemberExpression GetProperty(ParameterExpression param, string fieldName)
    {
        Expression prop = param;
        foreach (var s in fieldName.Split('.'))
        {
            prop = Expression.PropertyOrField(prop, s);
        }
        return (MemberExpression)prop;
    }

}