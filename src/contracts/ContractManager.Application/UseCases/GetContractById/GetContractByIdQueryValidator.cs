using FluentValidation;

namespace ContractManager.Application.UseCases.GetContractById;
public class GetContractByIdQueryValidator : AbstractValidator<GetContractByIdQuery>
{
    public GetContractByIdQueryValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty();
    }
}
