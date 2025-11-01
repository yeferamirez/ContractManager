using AutoFixture;
using Bogus;
using ContractManager.Api.Models.Contract;
using ContractManager.Data.Entities.Enums;

namespace ContractManager.Integration.Tests.Builders;

public static class FakerExtensions
{
    public static CreateContractCommandModel GetCreateContractCommandModel(
        this Fixture fixture,
        ContractTypeEnum procedureType = ContractTypeEnum.Servicio)
    {
        var faker = new Faker();

        return fixture.Build<CreateContractCommandModel>()
            .With(x => x.ContractTypeId, procedureType)
            .With(x => x.Value, 100000)
            .With(x => x.ClientId, Guid.NewGuid())
            .Without(x => x.StartDate)
            .Without(x => x.EndDate)
            .With(x => x.DigitalFileUrl, faker.Internet.Url)
            .Create();
    }

}
