using ContractManager.Application.Constants;
using ContractManager.Application.Repositories;
using ContractManager.Application.Storage;
using ContractManager.Data.Entities;
using ContractManager.Data.Entities.Enums;
using ContractManager.Shared.Data;
using IdGen;
using MediatR;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ContractManager.Application.UseCases.CreateContract;
public class CreateContractCommand : IRequest<CreatedContractResponseModel>
{
    public Guid ClientId { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public ContractTypeEnum ContractTypeId { get; set; }
    public double Value { get; set; }
    public string DigitalFileUrl { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int UserId { get; set; }
}

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, CreatedContractResponseModel>
{
    private readonly TimeProvider timeProvider;
    private readonly IContractRepository contractRepository;
    private readonly IDynamoRepository<AuditLog> auditRepository;
    private readonly IDynamoRepository<ContractDocument> documentRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IdGeneratorOptions idGeneratorOptions;

    public CreateContractCommandHandler(
        TimeProvider timeProvider,
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork,
        IdGeneratorOptions idGeneratorOptions,
        IDynamoRepository<AuditLog> auditRepository,
        IDynamoRepository<ContractDocument> documentRepository)
    {
        this.timeProvider = timeProvider;
        this.contractRepository = contractRepository;
        this.unitOfWork = unitOfWork;
        this.idGeneratorOptions = idGeneratorOptions;
        this.auditRepository = auditRepository;
        this.documentRepository = documentRepository;
    }

    public async Task<CreatedContractResponseModel> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = new Contract
        {
            ClientId = request.ClientId,
            ContractTypeId = (byte)request.ContractTypeId,
            Value = request.Value,
            DigitalFileUrl = request.DigitalFileUrl,
            StartDate = timeProvider.GetLocalNow().UtcDateTime,
            EndDate = request.EndDate,
            TrackingId = GetTrackingId().ToString(),
            Status = ContractStatus.Active
        };

        await this.contractRepository.InsertAsync(contract);

        await this.unitOfWork.SaveChangesAsync(cancellationToken);

        await this.auditRepository.SaveAsync(new AuditLog
        {
            Id = Guid.NewGuid().ToString(),
            User = contract.ClientId.ToString(),
            Action = AuditActions.CREATE_CONTRACT,
            Entity = JsonConvert.SerializeObject(contract),
            EntityId = contract.Id.ToString(),
            CreatedAt = timeProvider.GetLocalNow().UtcDateTime
        });

        await this.documentRepository.SaveAsync(new ContractDocument
        {
            ContractId = contract.Id.ToString(),
            DigitalFileUrl = request.DigitalFileUrl,
            UploadedAt = timeProvider.GetLocalNow().UtcDateTime
        });

        var createdContractResponse = new CreatedContractResponseModel
        {
            Id = contract.Id,
            TrackingId = contract.TrackingId
        };

        return createdContractResponse;
    }

    private long GetTrackingId()
    {
        var generator = new IdGenerator(1, idGeneratorOptions);
        return generator.CreateId();
    }
}