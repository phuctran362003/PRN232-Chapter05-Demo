using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Repository.Entities;
using Service.Interfaces;

namespace Presentaion.Controllers
{
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
            return await _watercolorsPaintingService.Search(date, author);
        }
        [HttpGet]
        [EnableQuery]
        public async Task<IEnumerable<WatercolorsPainting>> Get()
        {
            return await _watercolorsPaintingService.GetAll();
        }


        [HttpGet("{id}")]
        public async Task<WatercolorsPainting> Get(string id)
        {
            return await _watercolorsPaintingService.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(WatercolorsPainting transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _watercolorsPaintingService.CreateWithValidation(transaction);
            if (result.Contains("Thêm Thành công"))
            {
                return Ok(new
                {
                    Message = "Create successful",
                    Data = result
                });
            }
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
}
