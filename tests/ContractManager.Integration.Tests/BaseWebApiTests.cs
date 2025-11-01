using ContractManager.Application.Storage;
using ContractManager.Data.EF;
using ContractManager.Data.Entities;
using ContractManager.Shared.Application.Security.Configuration;
using ContractManager.Shared.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using Testcontainers.MsSql;

namespace ContractManager.Integration.Tests;
internal abstract class BaseWebApiTests : BaseIntegrationTests
{
    private MsSqlContainer sqlContainer;

    public BaseWebApiTests()
    {
        sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong!Passw0rd")
            .Build();
    }

    protected ApiApplicationFactory Factory { get; private set; }

    protected HttpClient Client { get; private set; }

    protected IConfiguration Configuration { get; private set; }

    protected Mock<IDynamoRepository<AuditLog>> AuditLopgStorageServiceMock => 
        Mock.Get(Factory.Services.GetRequiredService<IDynamoRepository<AuditLog>>());
    protected Mock<IDynamoRepository<ContractDocument>> ContractDocumentStorageServiceMock => 
        Mock.Get(Factory.Services.GetRequiredService<IDynamoRepository<ContractDocument>>());

    protected JwtSettings JwtSettings { get; private set; }

    protected ContractsDbContext DbContext => Factory.Services.GetRequiredService<ContractsDbContext>();

    [OneTimeSetUp]
    public async Task InitiateServer()
    {
        await sqlContainer.StartAsync();

        Factory = new ApiApplicationFactory(sqlContainer);
        Configuration = Factory.Services.GetRequiredService<IConfiguration>();
        JwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

        await InitDatabase();

        await Task.Delay(2000); // Temporary change to wait until background services are started
    }

    private async Task InitDatabase()
    {
        var context = Factory.Services.GetService<ContractsDbContext>();
        await context!.Database.EnsureCreatedAsync();
    }

    [SetUp]
    public void SetUp()
    {
        Client = Factory.CreateCustomClient();
        AuditLopgStorageServiceMock.Reset();
        ContractDocumentStorageServiceMock.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await sqlContainer.DisposeAsync();
        await Factory.DisposeAsync();
    }
}
