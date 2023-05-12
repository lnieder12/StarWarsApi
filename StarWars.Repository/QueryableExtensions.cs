using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace StarWars.Repository;

public static class QueryableExtensions
{

    public static IQueryable<T> AddFiltersSorted<T>(this IQueryable<T> query, Dictionary<string, StringValues> queryParams)
    {
        foreach (var kvp in queryParams)
        {
            if (kvp.Key.Equals("sort"))
            {
                var type = kvp.Value.ToString().Split(':');
                if (type[1] == "desc")
                {
                    query = query.OderByDynamic(type[0]);
                }
                else
                {
                    query = query.OderByDynamic(type[0], false);
                }
            }
            else if (!(kvp.Key.Equals("limit") || kvp.Key.Equals("skip")))
            {
                if (kvp.Value.ToString().Contains(':'))
                {
                    query = query.OperationFilters(kvp.Key, kvp.Value);
                }
                else
                {
                    query = query.FilterDynamic(kvp.Key, kvp.Value, "");
                }
            }
            
        }

        return query;

    }

    public static IQueryable<T> OperationFilters<T>(this IQueryable<T> query, string field, string values)
    {
        var operations = values.Split(',');
        foreach (var op in operations)
        {
            var split = op.Split(':');
            query = query.FilterDynamic(field, split[1], split[0]);
        }

        return query;
    }

    public static IQueryable<T> FilterDynamic<T>(this IQueryable<T> query, string fieldName, string values, string op)
    {
        
        var param = Expression.Parameter(typeof(T), "x");
        var prop = GetProperty(param, fieldName);

        var body = CreateBody(prop, values, op);
        var predicate = Expression.Lambda<Func<T, bool>>(body, param);
        
        return query.Where(predicate);
    }

    public static Expression CreateBody(MemberExpression prop, string value, string op)
    {
        switch (op)
        {
            case "lt":
                return Expression.LessThan(prop, Expression.Constant(int.Parse(value)));
            case "lte":
                return Expression.LessThanOrEqual(prop, Expression.Constant(int.Parse(value)));
            case "gt":
                return Expression.GreaterThan(prop, Expression.Constant(int.Parse(value)));
            case "gte":
                return Expression.GreaterThanOrEqual(prop, Expression.Constant(int.Parse(value)));
            default:
                var propertyInfo = (PropertyInfo)prop.Member;
                var type = propertyInfo.PropertyType;
                var method = type.GetMethod("Contains", new[] { type });
                var body = Expression.Call(prop, method,
                    Expression.Constant(value, type));
                return body;

        }
    }

    public static IQueryable<T> OderByDynamic<T>(this IQueryable<T> query, string fieldName, bool ascending = true)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var prop = GetProperty(param, fieldName);
        var lambda = Expression.Lambda(prop, param);
        var methodName = ascending ? "OrderBy" : "OrderByDescending";
        var method = typeof(Queryable).GetMethods()
            .First(method => method.Name == methodName && method.IsGenericMethodDefinition &&
                             method.GetGenericArguments().Length == 2 &&
                             method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), prop.Type);
        return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda });
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