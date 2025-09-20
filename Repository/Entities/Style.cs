using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Repository.Entities;

public class Style
{
    [Key] public string StyleId { get; set; } = null!;

    public string StyleName { get; set; } = null!;

    public string StyleDescription { get; set; } = null!;

    public string? OriginalCountry { get; set; }

    [JsonIgnore]
    public virtual ICollection<WatercolorsPainting> WatercolorsPaintings { get; set; } =
        new List<WatercolorsPainting>();
}