using Microsoft.EntityFrameworkCore;

namespace ECommerce.Helpers
{
    public static class HttpContextExtension
    {
        public async static Task InsertParamtersPaginationInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }
            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Add("TotalNumberOfRecords", count.ToString());
        }
    }
}
