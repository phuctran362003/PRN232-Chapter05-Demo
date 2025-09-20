using Repository.Entities;

namespace Service.Interfaces;

public interface IWatercolorsPaintingService
{
    Task<List<WatercolorsPainting>> GetAll();
    Task<WatercolorsPainting> GetById(string id);
    Task<List<WatercolorsPainting>> Search(int? item1, string? item2);
    Task<int> Create(WatercolorsPainting watercolorsPainting);
    Task<string> CreateWithValidation(WatercolorsPainting watercolorsPainting);

    Task<bool> Delete(string id);
    //Task<string> UpdateWithValidation(WatercolorsPainting watercolorsPainting);
}