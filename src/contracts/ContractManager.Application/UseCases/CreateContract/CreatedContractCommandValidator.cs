using FluentValidation;

namespace ContractManager.Application.UseCases.CreateContract;
public class CreatedContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreatedContractCommandValidator()
    {
        RuleFor(c => c.ClientId)
            .NotEmpty();

        RuleFor(c => c.ContractTypeId)
            .NotEmpty()
            .IsInEnum();

        RuleFor(c => c.Value)
            .NotEmpty()
            .InclusiveBetween(0, double.MaxValue);
    }
}
