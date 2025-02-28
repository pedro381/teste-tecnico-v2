using FluentValidation;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Domain.Enums;

namespace Thunders.TechTest.ApiService.Validators
{
    public class TollUsageValidator : AbstractValidator<TollUsage>
    {
        public TollUsageValidator()
        {
            RuleFor(x => x.UsageDateTime).NotEmpty();
            RuleFor(x => x.TollPlaza).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.State)
                .NotEmpty()
                .Length(2).WithMessage("O campo 'State' deve conter 2 caracteres (código do estado).");
            RuleFor(x => x.AmountPaid)
                .GreaterThan(0).WithMessage("O valor pago deve ser maior que zero.");
            RuleFor(x => x.VehicleType)
                .NotEmpty()
                .Must(v => v == VehicleType.Motorcycle || v == VehicleType.Car || v == VehicleType.Truck)
                .WithMessage("Tipo de veículo deve ser 'Moto', 'Carro' ou 'Caminhão'.");
        }
    }
}
