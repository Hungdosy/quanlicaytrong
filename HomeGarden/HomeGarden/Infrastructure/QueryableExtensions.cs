using HomeGarden.Dtos.Common;
using System.Linq.Expressions;

namespace HomeGarden.Infrastructure
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedAsync<T>(
            this IQueryable<T> q, PagedRequest p, CancellationToken ct = default)
        {
            var page = p.Page <= 0 ? 1 : p.Page;
            var size = p.Size <= 0 ? 20 : (p.Size > 200 ? 200 : p.Size);

            var total = await Task.Run(() => q.LongCount(), ct);
            var items = await Task.Run(() => q.Skip((page - 1) * size).Take(size).ToList(), ct);

            return new PagedResult<T> { Page = page, Size = size, Total = total, Items = items };
        }

        public static IQueryable<T> DynamicOrderBy<T>(this IQueryable<T> q, string? prop, bool desc)
        {
            if (string.IsNullOrWhiteSpace(prop)) return q;
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.PropertyOrField(param, prop);
            var lambda = Expression.Lambda(body, param);
            var method = desc ? "OrderByDescending" : "OrderBy";
            var types = new Type[] { typeof(T), body.Type };
            var m = typeof(Queryable).GetMethods()
                .Single(mi => mi.Name == method && mi.GetParameters().Length == 2)
                .MakeGenericMethod(types);
            return (IQueryable<T>)m.Invoke(null, new object[] { q, lambda })!;
        }
    }
}
