using CHSMS.API.DTOs;
using CHSMS.API.DTOs.MedicalSupplyConsumption;
using CHSMS.API.DTOs.UseMedicalSupply;

namespace CHSMS.API.Services.Interfaces
{
    public interface IUseMedicalSupplyService
    {
        Task<int> CreateUseMedicalSupplyAsync(int userId, int medicalRecordHistoryId, CreateUseMedicalSupplyDTO dto);
        Task EditUseMedicalSupplyForDoctorAsync(EditUseMedicalSupplyForDoctorDTO dto);
        Task EditUseMedicalSupplyForPharmacistAsync(EditUseMedicalSupplyForPharmacistDTO dto);
        Task<List<MedicalSupplyInventoryforUseDTO>> GetAllMedicalSuppliesInInventoryAsync();
        Task<List<UseMedicalSupplyDTO>> GetUseMedicalSuppliesByUserIdListAsync(int userId);
        Task<List<UseMedicalSupplyDTO>> GetAllUseMedicalSuppliesAsync();
        Task<List<UseMedicalSupplyDTO>> GetTodayUseMedicalSuppliesAsync();
        Task<List<UseMedicalSupplyDTO>> GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId);
        Task<UseMedicalSupplyDetailDTO> GetUseMedicalSupplyDetailAsync(int useMedicalSupplyId);
        Task<List<MedicalSupplyConsumptionStatisticDTO>> GetAllMedicalSupplyConsumptionsAsync();
    }
}
