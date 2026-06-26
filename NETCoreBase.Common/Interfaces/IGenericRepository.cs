using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NETCoreBase.Common.Interfaces
{
	public interface IGenericRepository<TEntity> : IDisposable where TEntity : class
	{
		List<T> ExecSQL<T>(string query, params System.Data.Common.DbParameter[] parameters);

		Task<int> SaveAsync();

		Task<List<T>> QueryAsync<T>(Expression<Func<TEntity, bool>> predicate);

		Task<PageResult<T>> QueryAsync<T>(Expression<Func<TEntity, bool>> predicate, QueryOption options);

		Task<T> SingleOrDefaultAsync<T>(Expression<Func<TEntity, bool>> predicate);

		Task<T> FirstOrDefaultAsync<T>(Expression<Func<TEntity, bool>> predicate);

		Task<T> InsertAsync<T>(T dto);

		Task<IEnumerable<T>> InsertRangeAsync<T>(IEnumerable<T> dtos);

		Task<int> UpdateAsync(TEntity entity);

		Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities);

		Task<int> DeleteAsync(TEntity entity);

		Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities);
	}
}
