using ECommerce.Pagination;
using System.Runtime.CompilerServices;

namespace ECommerce.Helpers
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, QueryStringParameters parametrs)
        {
            return queryable.Skip((parametrs.Page -1) * parametrs.RecordsPerPage)
                .Take(parametrs.RecordsPerPage);    
        }
    }
}
