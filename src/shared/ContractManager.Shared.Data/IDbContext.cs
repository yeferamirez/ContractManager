using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ContractManager.Shared.Data;
public interface IDbContext : IDisposable
{
    DatabaseFacade Database { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    EntityEntry Entry(object entity);
}
