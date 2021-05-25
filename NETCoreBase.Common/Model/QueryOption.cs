using System.Linq;
using System.Linq.Dynamic.Core;

namespace NETCoreBase.Common
{
	public class QueryOption
	{
        /// <summary>
        /// 查詢筆數
        /// </summary>
		public int? Top { get; set; }

        /// <summary>
        /// 略過筆數
        /// </summary>
		public int? Skip { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
		public string Orderby { get; set; }

		public IQueryable<T> ApplyTo<T>(IQueryable<T> query)
		{
			if (!string.IsNullOrWhiteSpace(Orderby))
			{
				query = query.OrderBy(Orderby);
			}
			if (Skip.HasValue)
			{
				query = query.Skip(Skip.Value);
			}
			if (Top.HasValue)
			{
				query = query.Take(Top.Value);
			}
			return query;
		}
	}
}
