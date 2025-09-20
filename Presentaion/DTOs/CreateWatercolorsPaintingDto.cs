using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentaion.DTOs
{
    public class CreateWatercolorsPaintingDto
    {
        [Required]
        [DefaultValue("Sunset Over Lake")]
        public string PaintingName { get; set; } = "Sunset Over Lake";

        [DefaultValue("A beautiful sunset landscape with vibrant colors.")]
        public string? PaintingDescription { get; set; } = "A beautiful sunset landscape with vibrant colors.";

        [DefaultValue("Nguyen Van A")]
        public string? PaintingAuthor { get; set; } = "Nguyen Van A";

        [DefaultValue(1500000.0)]
        public decimal? Price { get; set; } = 1500000.0m;

        [DefaultValue(2023)]
        public int? PublishYear { get; set; } = 2023;

        [DefaultValue(typeof(DateTime), "2023-05-20")]
        public DateTime? CreatedDate { get; set; } = new DateTime(2023, 5, 20);

        [Required]
        [DefaultValue("SS00112")]
        public string StyleId { get; set; } = "SS00112";
    }
}