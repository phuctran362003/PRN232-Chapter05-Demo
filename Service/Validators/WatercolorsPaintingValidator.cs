using FluentValidation;
using Repository.Entities;

namespace Service.Validators;

public class WatercolorsPaintingValidator : AbstractValidator<WatercolorsPainting>
{
    public WatercolorsPaintingValidator()
    {
        RuleFor(x => x.PaintingName)
            .NotEmpty().WithMessage("WaterColor is required.") 
            // Bắt buộc PaintingName có giá trị (không null, không rỗng)

            .Length(2, 80).WithMessage("WaterColor must be between 2 and 80 characters.") 
            // PaintingName phải có độ dài từ 2 đến 80 ký tự

            .Matches(@"^([A-Z][a-z0-9@#]*)(\s[A-Z][a-z0-9@#]*)*$")
            .WithMessage("Each word in WaterColor must begin with a capital letter and contain only valid characters.")
            // Mỗi từ trong PaintingName phải bắt đầu bằng chữ hoa, theo sau là chữ thường, số, @, # (nếu có)

            .Must(name => name == null || name.Trim() == name)
            .WithMessage("WaterColor must not have leading or trailing spaces.");
            // Không được có khoảng trắng thừa ở đầu hoặc cuối chuỗi


        RuleFor(x => x.PaintingAuthor)
            .NotEmpty().WithMessage("PaintingAuthor is required.") 
            // Bắt buộc PaintingAuthor có giá trị

            .Length(2, 80).WithMessage("PaintingAuthor must be between 2 and 80 characters.")
            // PaintingAuthor phải có độ dài từ 2 đến 80 ký tự

            .Matches(@"^([A-Z][a-z]+(?:\s[A-Z][a-z]+)*)$")
            .WithMessage("Each word in PaintingAuthor must begin with a capital letter and contain only letters.");
            // Mỗi từ trong PaintingAuthor phải bắt đầu bằng chữ hoa và chỉ chứa chữ cái


        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            // Giá trị Price phải > 0

            .LessThanOrEqualTo(1_000_000).WithMessage("Price must not exceed 1,000,000.")
            // Giá trị Price phải ≤ 1,000,000

            .Must(price => decimal.Round(price.Value, 2) == price)
            .WithMessage("Price must have at most 2 decimal places.");
            // Giá trị Price chỉ được phép có tối đa 2 chữ số thập phân
    }
}
