using System.Linq;
using NETCoreBase.Database;

namespace NETCoreBase.Common.Extensions
{
    /// <summary>
    /// LINQ extension methods for common query patterns.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Excludes soft-deleted entities (Status == "D") from the query.
        /// </summary>
        public static IQueryable<T> ExcludeDeleted<T>(this IQueryable<T> query)
            where T : ISoftDeletable
        {
            return query.Where(e => e.Status != "D");
        }
    }
}
