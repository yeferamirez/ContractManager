namespace ContractManager.Application.UseCases.CreateContract;
public class CreatedContractResponseModel
{
    public Guid Id { get; set; }
    public string TrackingId { get; set; } = default!;
}
