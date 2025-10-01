using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.WebApp.Models;

namespace RazorPage.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly string _apiUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7075/api";
    }

    public List<WatercolorsPainting> Paintings { get; set; } = new();
    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/WatercolorsPainting");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Paintings = JsonSerializer.Deserialize<List<WatercolorsPainting>>(content,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
                            new List<WatercolorsPainting>();

                // Set the Style object for each painting based on the StyleName property
                foreach (var painting in Paintings)
                    if (!string.IsNullOrEmpty(painting.StyleName))
                        painting.Style = new Style
                        {
                            StyleId = painting.StyleId ?? string.Empty,
                            StyleName = painting.StyleName
                        };
            }
            else
            {
                ErrorMessage = $"Failed to retrieve paintings. Status code: {response.StatusCode}";
                _logger.LogError(ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            _logger.LogError(ex, "Error retrieving paintings");
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/WatercolorsPainting/{id}");
            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Painting deleted successfully.";
            }
            else
            {
                ErrorMessage = $"Failed to delete painting. Status code: {response.StatusCode}";
                _logger.LogError(ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            _logger.LogError(ex, "Error deleting painting");
        }

        return RedirectToPage();
    }
}