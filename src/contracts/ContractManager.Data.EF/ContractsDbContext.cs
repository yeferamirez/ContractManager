using ContractManager.Data.Entities;
using ContractManager.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractManager.Data.EF;
public class ContractsDbContext : DbContext, IDbContext, IUnitOfWork
{
    public const string ContractSchema = "Contracts";

    public ContractsDbContext(DbContextOptions<ContractsDbContext> options) : base(options)
    {
        // IMPORTANT: To run new migration https://stackoverflow.com/a/66661099/4744829
    }

    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractType> ContractTypes { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ContractSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
