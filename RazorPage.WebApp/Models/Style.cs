namespace RazorPage.WebApp.Models
{
    public class Style
    {
        public string StyleId { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;
        public string StyleDescription { get; set; } = string.Empty;
        public string? OriginalCountry { get; set; }
    }
}