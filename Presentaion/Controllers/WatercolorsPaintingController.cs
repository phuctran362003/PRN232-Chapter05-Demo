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

    [HttpGet("search")]
    public async Task<IEnumerable<WatercolorsPainting>> Get(string? author, int? date)
    {
        var searchResults = await _watercolorsPaintingService.Search(date, author);
        
        // Sắp xếp các item theo CreatedDate giảm dần (mới nhất lên đầu)
        return searchResults.OrderByDescending(p => p.CreatedDate);
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IEnumerable<WatercolorsPainting>> Get()
    {
        var paintings = await _watercolorsPaintingService.GetAll();
        
        // Sắp xếp các item theo CreatedDate giảm dần (mới nhất lên đầu)
        return paintings.OrderByDescending(p => p.CreatedDate);
    }


    [HttpGet("{id}")]
    public async Task<WatercolorsPainting> Get(string id)
    {
        return await _watercolorsPaintingService.GetById(id);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateWatercolorsPaintingDto createDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Tạo mới đối tượng WatercolorsPainting từ DTO
        var watercolorsPainting = new WatercolorsPainting
        {
            // Tự động tạo ID (sử dụng timestamp để đảm bảo tính duy nhất)
            PaintingId = $"P{DateTime.Now:yyyyMMddHHmmss}",
            PaintingName = createDto.PaintingName,
            PaintingDescription = createDto.PaintingDescription,
            PaintingAuthor = createDto.PaintingAuthor,
            Price = createDto.Price,
            PublishYear = createDto.PublishYear,
            CreatedDate = createDto.CreatedDate ?? DateTime.Now,
            StyleId = createDto.StyleId
        };

        var result = await _watercolorsPaintingService.CreateWithValidation(watercolorsPainting);
        if (result.Contains("Thêm Thành công"))
            return Ok(new
            {
                Message = "Create successful",
                Data = result
            });
        return BadRequest(new
        {
            Message = "Validation failed",
            Errors = result
        });
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
    public async Task<bool> Delete(string id)
    {
        return await _watercolorsPaintingService.Delete(id);
    }
}