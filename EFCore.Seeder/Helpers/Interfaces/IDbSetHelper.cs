using Microsoft.EntityFrameworkCore;

namespace EFCore.Seeder.Helpers.Interfaces
{
    public interface IDbSetHelper
    {
        void AddOrUpdate<T>(DbSet<T> dbSet, T entity)
            where T : class;

        bool Any<T>(DbSet<T> dbSet) where T : class;
    }
}
