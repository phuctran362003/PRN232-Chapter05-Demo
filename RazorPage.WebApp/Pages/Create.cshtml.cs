using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.WebApp.Models;
using System.Text;
using System.Text.Json;

namespace RazorPage.WebApp.Pages
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public CreateModel(ILogger<CreateModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7075/api";
        }

        [BindProperty]
        public CreateWatercolorsPaintingDto PaintingDto { get; set; } = new CreateWatercolorsPaintingDto();

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // Initialize with default values if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var json = JsonSerializer.Serialize(PaintingDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}/WatercolorsPainting", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to create painting. Status code: {response.StatusCode}. Details: {errorContent}";
                    _logger.LogError(ErrorMessage);
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                _logger.LogError(ex, "Error creating painting");
                return Page();
            }
        }
    }
}