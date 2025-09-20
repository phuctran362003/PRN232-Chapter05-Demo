using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Repository.Entities;

public class WatercolorsPainting
{
    [Key] [DefaultValue("P001")] public string PaintingId { get; set; } = "P001";

    [DefaultValue("Sunset Over Lake")] public string PaintingName { get; set; } = "Sunset Over Lake";

    [DefaultValue("A beautiful sunset landscape with vibrant colors.")]
    public string? PaintingDescription { get; set; } = "A beautiful sunset landscape with vibrant colors.";

    [DefaultValue("Nguyen Van A")] public string? PaintingAuthor { get; set; } = "Nguyen Van A";

    [DefaultValue(1500000.0)] public decimal? Price { get; set; } = 1500000.0m;

    [DefaultValue(2023)] public int? PublishYear { get; set; } = 2023;

    [DefaultValue(typeof(DateTime), "2023-05-20")]
    public DateTime? CreatedDate { get; set; } = new DateTime(2023, 5, 20);

    [DefaultValue("STYLE01")] public string? StyleId { get; set; } = "STYLE01";

    [JsonIgnore]
    public virtual Style? Style { get; set; }
}