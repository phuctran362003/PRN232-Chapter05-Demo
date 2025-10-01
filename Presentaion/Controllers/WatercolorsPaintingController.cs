using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Presentaion.DTOs;
using Repository.Entities;
using Service.Interfaces;

namespace Presentaion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WatercolorsPaintingController : Controller
{
    private readonly IWatercolorsPaintingService _watercolorsPaintingService;

    public WatercolorsPaintingController(IWatercolorsPaintingService watercolorsPaintingService)
    {
        _watercolorsPaintingService = watercolorsPaintingService;
    }

    // Helper method to map Entity to DTO to avoid code duplication
    private WatercolorsPaintingResponseDto MapToDto(WatercolorsPainting painting)
    {
        return new WatercolorsPaintingResponseDto
        {
            PaintingId = painting.PaintingId,
            PaintingName = painting.PaintingName,
            PaintingDescription = painting.PaintingDescription,
            PaintingAuthor = painting.PaintingAuthor,
            Price = painting.Price,
            PublishYear = painting.PublishYear,
            CreatedDate = painting.CreatedDate,
            StyleId = painting.StyleId,
            StyleName = painting.Style?.StyleName
        };
    }

    [HttpGet("search")]
    public async Task<IEnumerable<WatercolorsPaintingResponseDto>> Get(string? author, int? date)
    {
        Console.WriteLine($"🔍 CONTROLLER: Search method called with author={author}, date={date}");
        Console.WriteLine(
            "🔄 ROUTING: Matched route 'api/WatercolorsPainting/search' with parameters from query string");
        Console.WriteLine(
            $"📦 MODEL BINDING: Parameters bound - author: '{author ?? "null"}', date: '{date?.ToString() ?? "null"}'");

        var stopwatch = Stopwatch.StartNew();
        var searchResults = await _watercolorsPaintingService.Search(date, author);
        stopwatch.Stop();

        Console.WriteLine($"⌛ PERFORMANCE: Search operation completed in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"📊 RESULT: Found {searchResults.Count()} matching painting(s)");

        // Sắp xếp các item theo CreatedDate giảm dần (mới nhất lên đầu)
        return searchResults.OrderByDescending(p => p.CreatedDate)
            .Select(MapToDto);
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IEnumerable<WatercolorsPaintingResponseDto>> Get()
    {
        var paintings = await _watercolorsPaintingService.GetAll();

        // Sắp xếp các item theo CreatedDate giảm dần (mới nhất lên đầu)
        return paintings.OrderByDescending(p => p.CreatedDate)
            .Select(MapToDto);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<WatercolorsPaintingResponseDto>> Get(string id)
    {
        Console.WriteLine($"🔍 CONTROLLER: Get by ID method called with id={id}");
        Console.WriteLine($"🔄 ROUTING: Matched route 'api/WatercolorsPainting/{id}' with parameter from route");
        Console.WriteLine($"📦 MODEL BINDING: Route value 'id' bound to parameter 'id' with value '{id}'");

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var painting = await _watercolorsPaintingService.GetById(id);
            stopwatch.Stop();

            Console.WriteLine($"⌛ PERFORMANCE: GetById operation completed in {stopwatch.ElapsedMilliseconds}ms");

            if (painting == null)
            {
                Console.WriteLine($"⚠️ NOT FOUND: Painting with ID {id} not found");
                return NotFound($"Painting with ID {id} not found");
            }

            Console.WriteLine($"✅ SUCCESS: Found painting with ID {id}, name: {painting.PaintingName}");
            return MapToDto(painting);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR: Exception occurred in Get(id) method: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateWatercolorsPaintingDto createDto)
    {
        Console.WriteLine(
            $"🔍 CONTROLLER: Post method called with DTO: PaintingName='{createDto.PaintingName}', Author='{createDto.PaintingAuthor}', StyleId='{createDto.StyleId}'");
        Console.WriteLine("🔄 ROUTING: Matched route 'api/WatercolorsPainting' with HTTP POST");
        Console.WriteLine("📦 MODEL BINDING: CreateWatercolorsPaintingDto bound from request body (FromBody)");

        // Improved model state validation with detailed error messages
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            Console.WriteLine($"❌ DATA ANNOTATION VALIDATION: Failed with {errors.Count} errors:");
            foreach (var error in errors) Console.WriteLine($"   - {error}");

            return BadRequest(new { Errors = errors });
        }

        Console.WriteLine("✅ DATA ANNOTATION VALIDATION: Passed for CreateWatercolorsPaintingDto");

        try
        {
            Console.WriteLine("🔄 PROCESSING: Mapping DTO to WatercolorsPainting entity");

            // Tạo mới đối tượng WatercolorsPainting từ DTO
            var watercolorsPainting = new WatercolorsPainting
            {
                // Using the service's ID generation method for consistency
                PaintingName = createDto.PaintingName,
                PaintingDescription = createDto.PaintingDescription,
                PaintingAuthor = createDto.PaintingAuthor,
                Price = createDto.Price,
                PublishYear = createDto.PublishYear,
                CreatedDate = createDto.CreatedDate ?? DateTime.Now,
                StyleId = createDto.StyleId
            };

            Console.WriteLine("🔍 FLUENT VALIDATION: Starting validation via service layer");

            // Use the CreateWithValidation method which performs FluentValidation
            var stopwatch = Stopwatch.StartNew();
            var result = await _watercolorsPaintingService.CreateWithValidation(watercolorsPainting);
            stopwatch.Stop();

            Console.WriteLine(
                $"⌛ PERFORMANCE: CreateWithValidation operation completed in {stopwatch.ElapsedMilliseconds}ms");

            // Check if result is an error message (string contains validation errors)
            if (result != "Thêm Thành công")
            {
                Console.WriteLine($"❌ FLUENT VALIDATION: Failed with result: {result}");
                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = result
                });
            }

            Console.WriteLine(
                $"✅ FLUENT VALIDATION: Passed, entity created successfully with ID: {watercolorsPainting.PaintingId}");
            return CreatedAtAction(nameof(Get), new { id = watercolorsPainting.PaintingId },
                MapToDto(watercolorsPainting));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR: Exception occurred in Post method: {ex.Message}");
            Console.WriteLine($"🔍 EXCEPTION DETAILS: {ex.StackTrace}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    //[HttpPut()]
    //[Authorize(Roles = "1")]
    //public async Task<IActionResult> Put(WatercolorsPainting watercolorsPainting)
    //{
    //    var result = await _watercolorsPaintingService.UpdateWithValidation(transaction);
    //    if (result.Contains("Edit thành công"))
    //    {
    //        return Ok(new
    //        {
    //            Message = "Edit successful",
    //            Data = result
    //        });
    //    }
    //    return BadRequest(new
    //    {
    //        Message = "Validation failed",
    //        Errors = result
    //    });
    //}

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var result = await _watercolorsPaintingService.Delete(id);
            if (result) return Ok(new { Message = $"Painting with ID {id} deleted successfully" });
            return NotFound(new { Message = $"Painting with ID {id} not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
        }
    }
}