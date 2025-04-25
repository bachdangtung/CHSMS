using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;

namespace CHSMS.API.Services.Interfaces
{
    public interface IMedicalSupplyService
    {
        List<MedicalSupplyDTO> GetAllMedicalSupplies();
        List<MedicalSupplyDTO> GetAllActualMedicalSupplies(DateTime? date);
        List<MedicalSupplyInventoryDTO>? GetMedicalSupplyById(int medicalSupplyId);
        List<MedicalSupplyInventoryDTO> MedicalSupplyInventoryByMedicalSupplyId(int medicalSupplyId);
        bool AddMedicalSupplyInventory(List<MedicalSupplyInventoryDTO> medicalSupplyInventoryDTO);
        bool UpdateMedicalSupplyInventory(MedicalSupplyInventoryDTO medicalSupplyInventoryDTO);
        int ConsumeMedicalSupply(ConsumpMSDTO consumpMSDTO);
        Dictionary<MedicalSupplyDTO, double> ConsumeReport(DateTime? from, DateTime? to);
        List<MedicalSupplyConsumption> ConsumptionDetail(int id, DateTime? from, DateTime? to);
        double GetAddOnMSI(int id, DateTime? from, DateTime? to);
        List<MedicalSupplyConsumption> ConsumptionHistory(DateTime? from, DateTime? to);
        MedicalSupply GetMedicalSupplyByMSIId(int id);
        bool UpdateMedicalSupplyConsumption(ConsumpMSDTO medicalSupplyConsumption);
        object GetExpiryMSI(int medicalSupplyId, DateTime? from, DateTime? to);
        MedicalSupplyInventory GetMedicalSupplyInventoryById(int? medicalSupplyInventoryId);
        List<MedicalSupplyInventory> GetMedicalSupplyImportHistory(DateTime fromDate, DateTime toDate);
        List<MedicalSupplyInventoryStatistic>? GetAllMedicalSupplyInventoryStatistics();
        List<MedicalSupplyInventoryStatistic>? GetMedicalSupplyInventoryStatisticsByStatisticDate(DateTime? from, DateTime? to);
        bool AddMedicalSupplyInventoryStatistic(List<MSIStatisticDTO> mSIStatisticDTOs);
        bool UpdateMedicalSupplyInventoryStatistic(List<MSIStatisticDTO> mSIStatisticDTOs);
        bool DeleteMedicalSupplyInventoryStatistic(int medicalSupplyInventoryStatisticId);


    }
}
