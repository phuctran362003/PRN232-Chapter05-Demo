using FluentValidation;
using Repository.Entities;

namespace Service.Validators;

public class WatercolorsPaintingValidator : AbstractValidator<WatercolorsPainting>
{
    public WatercolorsPaintingValidator()
    {
        Console.WriteLine("[FLUENT VALIDATOR] Initializing WatercolorsPaintingValidator with validation rules");

        // Set our cascade mode to continue on validation failures so all rules are evaluated
        CascadeMode = CascadeMode.Continue;
        RuleFor(x => x.PaintingName)
            .NotEmpty().WithMessage("Tên tranh là bắt buộc.")
            // Bắt buộc PaintingName có giá trị (không null, không rỗng)
            .Length(2, 80).WithMessage("Tên tranh phải có độ dài từ 2 đến 80 ký tự.")
            // PaintingName phải có độ dài từ 2 đến 80 ký tự
            .Matches(@"^([A-Z][a-z0-9@#]*)(\s[A-Z][a-z0-9@#]*)*$")
            .WithMessage("Mỗi từ trong tên tranh phải bắt đầu bằng chữ hoa và chỉ chứa chữ thường, số, @ hoặc #.")
            // Mỗi từ trong PaintingName phải bắt đầu bằng chữ hoa, theo sau là chữ thường, số, @, # (nếu có)
            .Must(name => name == null || name.Trim() == name)
            .WithMessage("Tên tranh không được có khoảng trắng ở đầu hoặc cuối.");
        // Không được có khoảng trắng thừa ở đầu hoặc cuối chuỗi


        RuleFor(x => x.PaintingAuthor)
            .NotEmpty().WithMessage("Tên tác giả là bắt buộc.")
            // Bắt buộc PaintingAuthor có giá trị
            .Length(2, 80).WithMessage("Tên tác giả phải có độ dài từ 2 đến 80 ký tự.")
            // PaintingAuthor phải có độ dài từ 2 đến 80 ký tự
            .Matches(@"^([A-Z][a-z]+(?:\s[A-Z][a-z]+)*)$")
            .WithMessage("Mỗi từ trong tên tác giả phải bắt đầu bằng chữ hoa và chỉ chứa chữ cái.");
        // Mỗi từ trong PaintingAuthor phải bắt đầu bằng chữ hoa và chỉ chứa chữ cái


        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá phải lớn hơn 0.")
            // Giá trị Price phải > 0
            .Must(price => price.HasValue && decimal.Round(price.Value, 2) == price)
            .WithMessage("Giá chỉ được phép có tối đa 2 chữ số thập phân.");
        // Giá trị Price chỉ được phép có tối đa 2 chữ số thập phân
    }
}