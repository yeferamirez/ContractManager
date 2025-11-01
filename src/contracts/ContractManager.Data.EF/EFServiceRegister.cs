using Ardalis.GuardClauses;
using ContractManager.Application.Repositories;
using ContractManager.Data.EF.Repositories;
using ContractManager.Data.Entities;
using ContractManager.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContractManager.Data.EF;

public static class EFServiceRegister
{
    public static void RegisterDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        var sqlServiceConnectionString = configuration["ConnectionStrings:DefaultConnection"];
        Guard.Against.Null(sqlServiceConnectionString,
            message: "Connection string 'ConnectionStrings:DefaultConnection' not found.");

        services.AddDbContext<ContractsDbContext>(options =>
            options.UseSqlServer(
                    sqlServiceConnectionString, x =>
                    {
                        x.MigrationsHistoryTable("__EFMigrationsHistory", ContractsDbContext.ContractSchema);
                    })
                .EnableDetailedErrors()
                .UseSeeding((dbContext, _) => SeedData(dbContext, isDevelopment)));

        services.AddScoped<IDbContext, ContractsDbContext>();

        services.AddScoped<IUnitOfWork>((sp) => sp.GetRequiredService<ContractsDbContext>());

        services.AddScoped<IContractRepository, ContractRepository>();
    }

    private static void SeedData(DbContext context, bool isDevelopment)
    {
        if (isDevelopment)
        {
            var db = (ContractsDbContext)context;

            if (!db.Roles.Any())
            {
                var adminRole = new Role { Name = "Admin" };
                var analystRole = new Role { Name = "Analyst" };

                var permissions = new List<Permission>
                {
                    new Permission { Name = "ContractRead" },
                    new Permission { Name = "ContractCreate" },
                    new Permission { Name = "ContractUpdate" },
                    new Permission { Name = "ContractDelete" },
                };

                db.Roles.AddRange(adminRole, analystRole);
                db.Permissions.AddRange(permissions);
                db.SaveChanges();

                var adminRoleId = db.Roles.First(r => r.Name == "Admin").Id;
                var analystRoleId = db.Roles.First(r => r.Name == "Analyst").Id;

                var perms = db.Permissions.ToList();

                db.RolePermissions.AddRange(
                    new RolePermission { RoleId = adminRoleId, PermissionId = perms.First(p => p.Name == "ContractRead").Id },
                    new RolePermission { RoleId = adminRoleId, PermissionId = perms.First(p => p.Name == "ContractCreate").Id },
                    new RolePermission { RoleId = adminRoleId, PermissionId = perms.First(p => p.Name == "ContractUpdate").Id },
                    new RolePermission { RoleId = adminRoleId, PermissionId = perms.First(p => p.Name == "ContractDelete").Id },
                    new RolePermission { RoleId = analystRoleId, PermissionId = perms.First(p => p.Name == "ContractRead").Id }
                );

                db.SaveChanges();
            }

            if (!db.ContractTypes.Any())
            {
                var contractTypes = new List<ContractType>
                {
                    new ContractType { Id = 1, Name = "Compra" },
                    new ContractType { Id = 2, Name = "Servicio" },
                    new ContractType { Id = 3, Name = "Mantenimiento" },
                    new ContractType { Id = 4, Name = "Consultoría" }
                };

                db.ContractTypes.AddRange(contractTypes);
                db.SaveChanges();
            }

            if (!db.UserRoles.Any())
            {
                // Simulando usuarios del sistema
                var defaultAdminUserId = 1;
                var defaultAnalystUserId = 2;

                var adminRoleId = db.Roles.First(r => r.Name == "Admin").Id;
                var analystRoleId = db.Roles.First(r => r.Name == "Analyst").Id;

                db.UserRoles.AddRange(
                    new UserRole { UserId = defaultAdminUserId, RoleId = adminRoleId },
                    new UserRole { UserId = defaultAnalystUserId, RoleId = analystRoleId }
                );

                db.SaveChanges();
            }
        }
    }
}
