using ContractManager.Shared.Core;
using Microsoft.EntityFrameworkCore;

namespace ContractManager.Shared.Data;

public class EFRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly IDbContext context;

    private DbSet<T> entities;

    public EFRepository(IDbContext context)
    {
        this.context = context;
    }

    public virtual IQueryable<T> Table => this.Entities;

    public IQueryable<T> TableNoTracking => this.Entities.AsNoTracking();

    protected virtual DbSet<T> Entities
    {
        get
        {
            if (this.entities == null)
            {
                this.entities = this.context.Set<T>();
            }

            return this.entities;
        }
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<int> DeleteAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        this.context.Entry(entity).State = EntityState.Deleted;
        this.Entities.Remove(entity);

        return await this.context.SaveChangesAsync();
    }

    public virtual async Task InsertAsync(IEnumerable<T> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        await this.Entities.AddRangeAsync(entities);

        await this.context.SaveChangesAsync();
    }

    public async Task InsertAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        this.Entities.Add(entity);

        await this.context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        this.context.Entry(entity).State = EntityState.Modified;

        return await this.context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(ICollection<T> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        foreach (var entity in entities)
        {
            this.context.Entry(entity).State = EntityState.Modified;
        }

        await this.context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await this.Entities.AsNoTracking().ToListAsync();
    }
}