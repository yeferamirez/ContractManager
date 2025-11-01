using ContractManager.Application.Storage;
using ContractManager.Data.EF;
using ContractManager.Data.Entities;
using ContractManager.Shared.Api.Constants;
using ContractManager.Shared.Sdk.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.MsSql;

namespace ContractManager.Integration.Tests;
internal class ApiApplicationFactory : WebApplicationFactory<Program>
{
    private readonly MsSqlContainer sqlContainer;

    public ApiApplicationFactory(MsSqlContainer sqlContainer)
    {
        this.sqlContainer = sqlContainer;
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment(ConfigurationApiConstants.IntegrationTestsEnvironment)
            .ConfigureTestServices(services =>
            {
                var dynamoLogRepository = Mock.Of<IDynamoRepository<AuditLog>>();
                var dynamoDocumentService = Mock.Of<IDynamoRepository<ContractDocument>>();

                services.AddTransient<LoggingHandler>();

                services.AddSingleton<IDynamoRepository<AuditLog>>(dynamoLogRepository);
                services.AddSingleton<IDynamoRepository<ContractDocument>>(dynamoDocumentService);

                AddTestContainerDatabase(services);
            });
    }

    private void AddTestContainerDatabase(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ContractsDbContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<ContractsDbContext>(options =>
        {
            var c = this.sqlContainer.GetConnectionString();
            options.UseSqlServer(this.sqlContainer.GetConnectionString());
            options.UseAsyncSeeding(SeedData);
        }, ServiceLifetime.Scoped);
    }

    private async Task SeedData(DbContext context, bool arg2, CancellationToken token)
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
            await db.SaveChangesAsync();

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

            await db.SaveChangesAsync();
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
            await db.SaveChangesAsync();
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

            await db.SaveChangesAsync();
        }
    }

    public HttpClient CreateCustomClient()
    {
        var handler = Services.GetRequiredService<LoggingHandler>();
        return CreateDefaultClient(new DelegatingHandler[] { handler });
    }
}
