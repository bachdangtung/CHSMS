using CHSMS.API.DTOs.Medicine;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;

namespace CHSMS.API.Services.Interfaces
{
    public interface IMedicineService
    {
        List<MedicineDTO> GetAllMedicine();
        MedicineDTO? GetMedicineById(int medicineId);
        List<MedicineInventoryDetailDTO> MedicineDetail(int medicineId);
        List<MedicineDTO> SearchMedicineByName(string name);
        Task<List<MedicineDTO>> SearchMedicinesAsync(
            int? medicineId = null,
            string? medicineName = null,
            string? activeIngredient = null,
            string? dosage = null,
            string? dosageForm = null,
            double? quantity = null,
            double? importPrice = null,
            DateTime? expiryDate = null,
            string? batchNumber = null,
            string? bidNumber = null,
            bool? status = null,
            DateTime? minExpiryDate = null,
            DateTime? maxExpiryDate = null);
        AddMedicineInventoryResultDTO AddMedicineInventoryList(List<MedicineInventoryAddDTO> dtoList, int userId);
        bool UpdateMedicineInventory(MedicineInventoryUpdateDTO dto, int userId);
        List<MedicineInventory> GetRecentInventoryHistory(int userId);
        List<MedicineInventoryUpdateHistoryDTO> GetAllInventoryHistory(int userId);
        List<MedicineDTO> FilterMedicineStock(MedicineInventoryFilter filter);
        List<MedicineInventoryGetAllDTO> GetAllMedicineInInventory();
        List<MedicineDTO> GetAllActualMedicines(DateTime? date);
        List<SupplierDTO> GetAllSuppliers();
        List<UserDTO> GetAllReceivers();
        List<MedicineInventoryDetailDTO> GetMedicineInventoryByMedicineId(int medicineId);
        int ConsumeMedicine(ConsumeMedicineDTO consumeMedicineDTO);
        Dictionary<MedicineDTO, double> ConsumeReport(DateTime? from, DateTime? to);
        object GetExpiryMedicineInventory(int medicineId, DateTime? from, DateTime? to);
        double GetAddOnMedicineInventory(int id, DateTime? from, DateTime? to);
        List<MedicineConsumption> ConsumptionDetail(int id, DateTime? from, DateTime? to);
        List<MedicineConsumption> ConsumptionHistory(DateTime? from, DateTime? to);
        Medicine GetMedicineByMedicineInventoryId(int medicineInventoryId);
        MedicineInventory GetMedicineInventoryById(int? medicineInventoryId);
        bool UpdateMedicineConsumption(ConsumeMedicineDTO medicineConsumption);
        List<MedicineInventory>? GetMedicineImportHistory(DateTime fromDate, DateTime toDate);
        List<MedicineInventoryStatistic>? GetAllMedicineInventoryStatistics();
        List<MedicineInventoryStatistic>? GetMedicineInventoryStatisticsByStatisticDate(DateTime? from, DateTime? to);
        bool AddMedicineInventoryStatistic(List<MedicineInventoryStatisticDTO> mIStatisticDTOs);
        bool UpdateMedicineInventoryStatistic(List<MedicineInventoryStatisticDTO> mIStatisticDTOs);
        bool DeleteMedicineInventoryStatistic(int medicineInventoryStatisticId);
    }
}
