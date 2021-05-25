using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using NETCoreBase.Common.Extensions;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Database;
using Microsoft.EntityFrameworkCore;
using NETCoreBase.Database.Models;

namespace NETCoreBase.Common.Services
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity>, IDisposable where TEntity : class
	{
		private NETCoreBaseContext _context;

		private readonly IMapper _mapper;

		private readonly ClaimsPrincipal _clamis;

		public GenericRepository(NETCoreBaseContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context fail");
			}
		}

		public GenericRepository(NETCoreBaseContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public GenericRepository(NETCoreBaseContext context, IMapper mapper, ClaimsPrincipal clamis)
		{
			_context = context;
			_mapper = mapper;
			_clamis = clamis;
		}

		protected virtual IQueryable<TEntity> QueryProcess()
		{
			return _context.Set<TEntity>().AsNoTracking();
		}

		protected virtual void InsertProcess(TEntity entity)
		{
			_context.Entry(entity).State = EntityState.Added;
		}

		protected virtual void UpdateProcess(TEntity entity)
		{
			_context.Entry(entity).State = EntityState.Modified;
		}

		protected virtual void DeleteProcess(TEntity entity)
		{
			_context.Entry(entity).State = EntityState.Deleted;
		}

		protected virtual async Task<int> SaveChangesAsync()
		{
			if (_clamis != null)
			{
				DbContextExtensions.SaveChangesProcess(user: _clamis.Identity!.Name, dbContext: _context);
			}
			return await _context.SaveChangesAsync();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _context != null)
			{
				_context.Dispose();
				_context = null;
			}
		}

		public List<T> ExecSQL<T>(string query)
		{
			using DbCommand dbCommand = _context.Database.GetDbConnection().CreateCommand();
			dbCommand.CommandText = query;
			dbCommand.CommandType = CommandType.Text;
			_context.Database.OpenConnection();
			List<T> list = new List<T>();
			using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
			{
				T val = default(T);
				while (dbDataReader.Read())
				{
					val = Activator.CreateInstance<T>();
					PropertyInfo[] properties = val.GetType().GetProperties();
					foreach (PropertyInfo propertyInfo in properties)
					{
						if (!object.Equals(dbDataReader[propertyInfo.Name], DBNull.Value))
						{
							propertyInfo.SetValue(val, dbDataReader[propertyInfo.Name], null);
						}
					}
					list.Add(val);
				}
			}
			_context.Database.CloseConnection();
			return list;
		}

		public async Task<List<T>> QueryAsync<T>(Expression<Func<TEntity, bool>> predicate)
		{
			IQueryable<TEntity> query = QueryProcess();
			if (predicate != null)
			{
				query = query.Where(predicate);
			}
			return await _mapper.ProjectTo(query, null, Array.Empty<Expression<Func<T, object>>>()).ToListAsync();
		}

		public async Task<PageResult<T>> QueryAsync<T>(Expression<Func<TEntity, bool>> predicate, QueryOption options)
		{
			IQueryable<TEntity> query = QueryProcess();
			if (predicate != null)
			{
				query = query.Where(predicate);
			}
			if (options == null)
			{
				options = new QueryOption();
			}
			return new PageResult<T>(await _mapper.ProjectTo(options.ApplyTo(query), null, Array.Empty<Expression<Func<T, object>>>()).ToListAsync(), await query.CountAsync());
		}

		public async Task<T> SingleOrDefaultAsync<T>(Expression<Func<TEntity, bool>> predicate)
		{
			IQueryable<TEntity> query = QueryProcess();
			if (predicate != null)
			{
				query = query.Where(predicate);
			}
			return await _mapper.ProjectTo(query, null, Array.Empty<Expression<Func<T, object>>>()).SingleOrDefaultAsync();
		}

		public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<TEntity, bool>> predicate)
		{
			IQueryable<TEntity> query = QueryProcess();
			if (predicate != null)
			{
				query = query.Where(predicate);
			}
			return await _mapper.ProjectTo(query, null, Array.Empty<Expression<Func<T, object>>>()).FirstOrDefaultAsync();
		}

		public async Task<T> InsertAsync<T>(T dto)
		{
			TEntity entity = _mapper.Map<TEntity>(dto);
			InsertProcess(entity);
			await SaveChangesAsync();
			return _mapper.Map<T>(entity);
		}

		public async Task<IEnumerable<T>> InsertRangeAsync<T>(IEnumerable<T> dtos)
		{
			if (!dtos.Any())
			{
				return new List<T>();
			}
			List<TEntity> entities = _mapper.Map<List<TEntity>>(dtos.ToList());
			foreach (TEntity entity in entities)
			{
				InsertProcess(entity);
			}
			await SaveChangesAsync();
			return _mapper.Map<List<T>>(entities.ToList());
		}

		public async Task<int> UpdateAsync(TEntity entity)
		{
			UpdateProcess(entity);
			return await SaveChangesAsync();
		}

		public async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
		{
			if (!entities.Any())
			{
				return 0;
			}
			foreach (TEntity entity in entities)
			{
				UpdateProcess(entity);
			}
			return await SaveChangesAsync();
		}

		public async Task<int> DeleteAsync(TEntity entity)
		{
			DeleteProcess(entity);
			return await SaveChangesAsync();
		}

		public async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities)
		{
			if (!entities.Any())
			{
				return 0;
			}
			foreach (TEntity entity in entities)
			{
				DeleteProcess(entity);
			}
			return await SaveChangesAsync();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
