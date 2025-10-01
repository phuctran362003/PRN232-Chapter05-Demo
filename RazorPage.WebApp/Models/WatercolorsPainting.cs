namespace RazorPage.WebApp.Models;

public class WatercolorsPainting
{
    public string PaintingId { get; set; } = string.Empty;
    public string PaintingName { get; set; } = string.Empty;
    public string? PaintingDescription { get; set; }
    public string? PaintingAuthor { get; set; }
    public decimal? Price { get; set; }
    public int? PublishYear { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? StyleId { get; set; }
    public string? StyleName { get; set; }
    public Style? Style { get; set; }
}