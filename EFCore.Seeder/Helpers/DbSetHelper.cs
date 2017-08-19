using System.Linq;
using EFCore.Seeder.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var dbEntity = dbSet.FirstOrDefault(dbSetEntity => dbSetEntity.Equals(entity));

            if (dbEntity == null)
                dbSet.Add(entity);
            else
                dbSet.Update(entity);
        }
    }
}
