using Microsoft.EntityFrameworkCore;

namespace BillingGrpcServer.Utils;

public static class QueryableToArray
{
    public static async ValueTask<TSource[]> GetArray<TSource>(this IQueryable<TSource> source)
    {
        TSource[] items;
        if (source is IAsyncEnumerable<TSource>)
        {
            items = await source.ToArrayAsync();
        }
        else
        {
            items = source.ToArray();
        }

        return items;
    }
}