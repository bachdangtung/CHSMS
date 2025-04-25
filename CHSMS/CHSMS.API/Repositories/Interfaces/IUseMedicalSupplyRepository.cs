using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IUseMedicalSupplyRepository
    {
        Task<UseMedicalSupply> CreateUseMedicalSupplyAsync(UseMedicalSupply useMedicalSupply);
        Task<MedicalSupplyInventory> GetMedicalSupplyInventoryByIdAsync(int SupplyInventoryId);
        Task<MedicalSupplyConsumption> CreateMedicalSupplyConsumptionAsync(MedicalSupplyConsumption consumption);
        Task<UseMedicalSuppliesMedicalSupplyConsumption> CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(UseMedicalSuppliesMedicalSupplyConsumption umsmsc);
        Task<List<MedicalSupplyInventory>> GetAvailableMedicalSuppliesAsync();
        Task UpdateMedicalSupplyInventoryAsync(MedicalSupplyInventory inventory);
        Task<List<UseMedicalSupply>> GetUseMedicalSuppliesByUserIdAsync(int userId);
        Task<List<UseMedicalSupply>> GetAllUseMedicalSuppliesAsync();
        Task<List<UseMedicalSupply>> GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId);
        Task<UseMedicalSupply> GetUseMedicalSupplyDetailAsync(int useMedicalSupplyId);
        Task<UseMedicalSupply> GetUseMedicalSupplyByIdAsync(int useMedicalSupplyId);
        Task<List<MedicalSupplyConsumption>> GetMedicalSupplyConsumptionsByUseMedicalSupplyIdAsync(int useMedicalSupplyId);
        Task UpdateUseMedicalSupplyAsync(UseMedicalSupply useMedicalSupply);
        Task<UseMedicalSuppliesMedicalSupplyConsumption> GetUseMedicalSuppliesMedicalSupplyConsumptionByConsumptionIdAsync(int msConsumptionId);
        Task DeleteUseMedicalSuppliesMedicalSupplyConsumptionAsync(int useMedicalSupplyId, int msConsumptionId);
        Task DeleteMedicalSupplyConsumptionAsync(int consumptionId);
        Task<MedicalSupplyConsumption> GetMedicalSupplyConsumptionByIdAsync(int msConsumptionId);
        Task UpdateMedicalSupplyConsumptionAsync(MedicalSupplyConsumption consumption);
        Task UpdateUseMedicalSuppliesMedicalSupplyConsumptionAsync(UseMedicalSuppliesMedicalSupplyConsumption umsmsc);
        Task<List<UseMedicalSuppliesMedicalSupplyConsumption>> GetAllMedicalSupplyConsumptionsAsync();
    }
}
