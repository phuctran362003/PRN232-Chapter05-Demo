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
        var searchResults = await _watercolorsPaintingService.Search(date, author);
        
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
        try
        {
            var painting = await _watercolorsPaintingService.GetById(id);
            if (painting == null)
            {
                return NotFound($"Painting with ID {id} not found");
            }
            return MapToDto(painting);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateWatercolorsPaintingDto createDto)
    {
        // Improved model state validation with detailed error messages
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(new { Errors = errors });
        }

        try
        {
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

            // Use the CreateWithValidation method which performs FluentValidation
            var result = await _watercolorsPaintingService.CreateWithValidation(watercolorsPainting);
            
            // Check if result is an error message (string contains validation errors)
            if (result != "Thêm Thành công")
            {
                return BadRequest(new { 
                    Message = "Validation failed",
                    Errors = result 
                });
            }
            
            return CreatedAtAction(nameof(Get), new { id = watercolorsPainting.PaintingId }, MapToDto(watercolorsPainting));
        }
        catch (Exception ex)
        {
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
            if (result)
            {
                return Ok(new { Message = $"Painting with ID {id} deleted successfully" });
            }
            return NotFound(new { Message = $"Painting with ID {id} not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
        }
    }
}