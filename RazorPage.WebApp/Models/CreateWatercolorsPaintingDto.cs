using System.ComponentModel.DataAnnotations;

namespace RazorPage.WebApp.Models;

public class CreateWatercolorsPaintingDto
{
    [Required(ErrorMessage = "Painting name is required")]
    [Display(Name = "Painting Name")]
    public string PaintingName { get; set; } = "Sunset Over Lake";

    [Display(Name = "Description")]
    public string? PaintingDescription { get; set; } = "A beautiful sunset landscape with vibrant colors.";

    [Display(Name = "Author")] public string? PaintingAuthor { get; set; } = "Nguyen Van A";

    [Display(Name = "Price")] public decimal? Price { get; set; } = 1500000.0m;

    [Display(Name = "Publish Year")] public int? PublishYear { get; set; } = 2023;

    [Display(Name = "Created Date")]
    [DataType(DataType.Date)]
    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Style ID is required")]
    [Display(Name = "Style ID")]
    public string StyleId { get; set; } = "SS00112";
}