using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.WebApp.Models;
using System.Text.Json;

namespace RazorPage.WebApp.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ILogger<DetailsModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public DetailsModel(ILogger<DetailsModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7075/api";
        }

        public WatercolorsPainting? Painting { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("./Index");
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/WatercolorsPainting/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Painting = JsonSerializer.Deserialize<WatercolorsPainting>(content, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (Painting == null)
                    {
                        ErrorMessage = "Unable to deserialize painting data.";
                        return Page();
                    }
                    
                    // Set the Style object based on the StyleName property
                    if (!string.IsNullOrEmpty(Painting.StyleName))
                    {
                        Painting.Style = new Style
                        {
                            StyleId = Painting.StyleId ?? string.Empty,
                            StyleName = Painting.StyleName
                        };
                    }
                    
                    return Page();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Painting not found.";
                    return Page();
                }
                else
                {
                    ErrorMessage = $"Failed to retrieve painting details. Status code: {response.StatusCode}";
                    _logger.LogError(ErrorMessage);
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                _logger.LogError(ex, "Error retrieving painting details");
                return Page();
            }
        }
    }
}