using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.WebApp.Models;

namespace RazorPage.WebApp.Pages;

public class SearchModel : PageModel
{
    private readonly string _apiUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SearchModel> _logger;

    public SearchModel(ILogger<SearchModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7075/api";
    }

    [BindProperty(SupportsGet = true)] public string? Author { get; set; }

    [BindProperty(SupportsGet = true)] public int? Date { get; set; }

    public List<WatercolorsPainting> Paintings { get; set; } = new();
    public string ErrorMessage { get; set; } = string.Empty;
    public bool SearchPerformed { get; set; }

    public async Task OnGetAsync()
    {
        // Only perform search if at least one parameter is provided
        if (!string.IsNullOrWhiteSpace(Author) || Date.HasValue)
        {
            SearchPerformed = true;

            try
            {
                var queryString = $"?author={Uri.EscapeDataString(Author ?? string.Empty)}";
                if (Date.HasValue) queryString += $"&date={Date.Value}";

                var response = await _httpClient.GetAsync($"{_apiUrl}/WatercolorsPainting/search{queryString}");

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
                    ErrorMessage = $"Failed to retrieve search results. Status code: {response.StatusCode}";
                    _logger.LogError(ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                _logger.LogError(ex, "Error searching paintings");
            }
        }
    }
}