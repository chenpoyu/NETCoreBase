using System.Collections.Generic;

namespace NETCoreBase.Common
{
	public class PageResult<T>
	{
		public IEnumerable<T> Items { get; set; } = new HashSet<T>();


		public int TotalCount { get; set; } = 0;


		public string ExtMsg { get; set; } = "";


		public PageResult()
		{
		}

		public PageResult(IEnumerable<T> items, int totalCount, string msg = "")
		{
			Items = items;
			TotalCount = totalCount;
			ExtMsg = msg;
		}
	}
}
