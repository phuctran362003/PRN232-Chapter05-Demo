using FluentValidation;
using Repository.Entities;
using Repository.Repositories;
using Service.Interfaces;

namespace Service.Services
{
    public class WatercolorsPaintingService : IWatercolorsPaintingService
    {
        private readonly WatercolorsPaintingRepo _repo;
        private readonly IValidator<WatercolorsPainting> _validator;

        public WatercolorsPaintingService(WatercolorsPaintingRepo repo, IValidator<WatercolorsPainting> validator)
        {
            _repo = repo;
            _validator = validator;
        }

        public Task<int> Create(WatercolorsPainting watercolorsPainting)
        {
            watercolorsPainting.CreatedDate = DateTime.Now;
            return _repo.CreateAsync(watercolorsPainting);
        }

        public async Task<string> CreateWithValidation(WatercolorsPainting watercolorsPainting)
        {
            // Kiểm tra dữ liệu với FluentValidation
            var validationResult = await _validator.ValidateAsync(watercolorsPainting);
            if (!validationResult.IsValid)
            {
                return string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            }

            watercolorsPainting.PaintingId = GenerateId();
            var result = await _repo.CreateAsync(watercolorsPainting);
            if (result == 1)
            {
                return "Thêm Thành công";
            }
            return "Thêm thất bại";
        }

        public string GenerateId()
        {
            return "WP" + DateTime.UtcNow.ToString("yyyyMMddHHmmss").Substring(0, 3) + Guid.NewGuid().ToString("N").Substring(0, 3);
        }

        public async Task<bool> Delete(string id)
        {
            var item = _repo.GetById(id);
            return await _repo.RemoveAsync(item);
        }

        public async Task<List<WatercolorsPainting>> GetAll()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<WatercolorsPainting> GetById(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<WatercolorsPainting>> Search(int? item1, string? item2)
        {
            return await _repo.Search(item1, item2);
        }


    }
}
