using AutoMapper;
using ContractManager.Api.Models.Contract;
using ContractManager.Application.UseCases.CreateContract;
using ContractManager.Application.UseCases.GetAllContracts;
using ContractManager.Application.UseCases.GetContractById;
using ContractManager.Shared.Api.Attributes;
using ContractManager.Shared.Api.Controllers;
using ContractManager.Shared.Application.Errors;
using ContractManager.Shared.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractManager.Api.Controllers.Contract;

[Route("api/v1/contracts")]
public class ContractsController : BaseApiController
{
    private readonly ISender sender;
    private readonly IMapper mapper;

    public ContractsController(
        ISender sender,
        IMapper mapper)
    {
        this.sender = sender;
        this.mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    [AuthorizationRole(PermissionType.ContractCreate)]
    [NonEmptyModel]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatedContractResponseModel))]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractCommandModel model)
    {
        var request = this.mapper.Map<CreateContractCommand>(model);
        request.UserId = this.CurrentUser!.Id;

        var createdContractResponse = await this.sender.Send(request);

        return this.Created("GetContract", createdContractResponse);
    }

    [HttpGet("{id:guid}", Name = "GetContract")]
    [Authorize]
    [AuthorizationRole(PermissionType.ContractRead)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetContractByIdQuery
        {
            ContractId = id
        };

        var result = await this.sender.Send(query);

        if (!result.IsSuccess)
        {
            if (result.HasError<NotFoundError>())
            {
                return this.NotFound();
            }

            return this.BadRequest(result.Errors);
        }

        return this.Ok(result.Value);
    }

    [HttpGet]
    [Authorize]
    [AuthorizationRole(PermissionType.ContractRead)]
    public async Task<IActionResult> GetAll([FromQuery] ContractFilterModel filter)
    {
        var query = new GetAllContractsQuery
        {
            Page = filter.Page,
            PageSize = filter.PageSize,
            EndDate = filter.EndDate,
            Status = filter.Status,
            ContractType = filter.ContractType,
            OrderBy = filter.OrderByEnum,
            SortDirection = filter.SortDirection
        };

        var result = await this.sender.Send(query);

        if (!result.IsSuccess)
        {
            return this.BadRequest(result.Errors);
        }

        return this.Ok(
            result.Value.ToArray(),
            result.Value.HasNextPage,
            result.Value.TotalCount);
    }
}
