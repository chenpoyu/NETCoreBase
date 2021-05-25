using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NETCoreBase.Common.Extensions
{
	public static class DbContextExtensions
	{
		public static void SaveChangesProcess(this DbContext dbContext, string user)
		{
			IEnumerable<EntityEntry> enumerable = from c in dbContext.ChangeTracker.Entries()
				where c.State == EntityState.Added || c.State == EntityState.Modified
				select c;
			foreach (EntityEntry item in enumerable)
			{
				if (item.State == EntityState.Added || item.State == EntityState.Modified)
				{
					ValidateObject(item.Entity);
					ApplyAuditable(item, user);
				}
			}
		}

		private static void ValidateObject(object entity)
		{
			ValidationContext validationContext = new ValidationContext(entity);
			Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
		}

		private static void ApplyAuditable(EntityEntry entry, string user)
		{
			DateTime now = DateTime.Now;
			object entity = entry.Entity;
			if (entry.State == EntityState.Added)
			{
				var temp = new {
                    CreateDate = DateTimeOffset.Now,
                    CreateUser = string.IsNullOrEmpty(user) ? "Guest" : user,
                };
                entry.CurrentValues.SetValues(temp);
			}
            entry.CurrentValues.SetValues(new {
                UpdateDate = DateTimeOffset.Now,
                UpdateUser = string.IsNullOrEmpty(user) ? "Guest" : user,
            });
		}
	}
}
