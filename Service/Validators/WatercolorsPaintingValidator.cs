using FluentValidation;
using Repository.Entities;

namespace Service.Validators;

public class WatercolorsPaintingValidator : AbstractValidator<WatercolorsPainting>
{
    public WatercolorsPaintingValidator()
    {
        RuleFor(x => x.PaintingName)
            .NotEmpty().WithMessage("WaterColor is required.")
            .Length(2, 80).WithMessage("WaterColor must be between 2 and 80 characters.")
            .Matches(@"^([A-Z][a-z0-9@#]*\s?)+$")
            .WithMessage("Each word in WaterColor must begin with a capital letter and contain only valid characters.");

        RuleFor(x => x.PaintingAuthor)
            .NotEmpty().WithMessage("PaintingAuthor is required.")
            .Length(2, 80).WithMessage("PaintingAuthor must be between 2 and 80 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}