using ContractManager.Api.Constants;
using ContractManager.Application.UseCases.CreateContract;
using ContractManager.Data.Entities.Enums;
using ContractManager.Integration.Tests.Builders;
using ContractManager.Integration.Tests.Extensions;
using ContractManager.Shared.Application.Security;
using Shouldly;
using System.Net;

namespace ContractManager.Integration.Tests.Controllers;

[TestFixture]
internal class CreateContractTests : BaseWebApiTests
{
    [Test]
    public async Task CreateProcedure_ValidModel_Return201()
    {
        var defaultProcedureTypeId = ContractTypeEnum.Servicio;

        var model = Fixture.GetCreateContractCommandModel(defaultProcedureTypeId);
        
        var response = await CreatePostRequest(ApiConstants.ContractsV1)
            .AddJwt(JwtSettings, [PermissionType.ContractCreate])
            .SendAsync<CreatedContractResponseModel>(Client, model);

        response.HttpResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
