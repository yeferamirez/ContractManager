using AutoFixture.NUnit3;
using AutoMapper;
using ContractManager.Api.Controllers.Contract;
using ContractManager.Api.Models.Contract;
using ContractManager.Api.Tests.Autofixture;
using ContractManager.Application.UseCases.CreateContract;
using ContractManager.Shared.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;

namespace ContractManager.Api.Tests.Controller;

public class ContractControllerTests : BaseApiTest
{
    [Test]
    [AutoMoqData]
    public async Task CreateContract_ValidaModel_ShouldPass(
        [Frozen] Mock<IMapper> mapper,
        [Frozen] Mock<ISender> sender, 
        [Frozen] Mock<IUrlHelper> urlHelper,
        CreateContractCommandModel model,
        CreateContractCommand command,
        Guid clientId,
        CreatedContractResponseModel responseModel)
    {
        var controller = new ContractsController(
            sender.Object,
            mapper.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.User = GetPrincipal();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        controller.Url = urlHelper.Object;

        mapper
            .Setup(x => x.Map<CreateContractCommand>(model))
            .Returns(command);

        command.ClientId = clientId;

        sender
            .Setup(x => x.Send(It.IsAny<CreateContractCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseModel);

        var result = await controller.CreateContract(model);

        sender.Verify(x => x.Send(It.Is<CreateContractCommand>(c => c.ClientId == clientId), It.IsAny<CancellationToken>()), Times.Once);

        result.ShouldBeOfType<CreatedResult>();
    }
}
