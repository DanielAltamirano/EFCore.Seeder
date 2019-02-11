using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EFCore.Seeder.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace EFCore.Seeder.Helpers
{
    // Created first of all in order to abstract the DbSet to an interface and make it testable
    // Also, EF Core lacks AddOrUpdate, so we have each entity implement IEquatable, so we can compare
    // them by the right fields in order to check if the entity exists in order to update it

    public class DbSetHelper : IDbSetHelper
    {
        public bool Any<T>(DbSet<T> dbSet) where T : class
        {
            return dbSet.Any();
        }

        public void AddOrUpdate<T>(DbSet<T> dbSet, T entity) where T : class
        {
            Expression<Func<T, bool>> predicate = dbSetEntity => dbSetEntity.Equals(entity);
            var dbEntity = dbSet.FirstOrDefault(predicate.Compile());
            
            if (dbEntity == null)
                dbSet.Add(entity);
            else
            {
                var infrastructure = dbSet as IInfrastructure<IServiceProvider>;
                var serviceProvider = infrastructure.Instance;

                if (serviceProvider.GetService(typeof(ICurrentDbContext)) is ICurrentDbContext currentDbContext)
                {
                    var keys = currentDbContext.Context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name).ToList();
                    Update(entity, dbEntity, keys);
                }
                
                dbSet.Update(entity);
            }
        }

        public void Update<T>(T o, T d, ICollection<string> keys) where T : class
        {
            var type = o.GetType();
            while (type != null)
            {
                UpdateForType(type, o, d, keys);
                type = type.BaseType;
            }
        }

        private static void UpdateForType<T>(IReflect type, T source, T destination, ICollection<string> keys) where T : class
        {
            var myObjectFields = type.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var fi in myObjectFields.Where(info => !keys.Contains(info.Name)))
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }
    }
}
