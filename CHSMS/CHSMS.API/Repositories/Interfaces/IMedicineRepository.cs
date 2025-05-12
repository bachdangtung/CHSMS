using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IMedicineRepository
    {
        List<Medicine> GetAllMedicine();
        List<MedicineInventoryDetailDTO> GetMedicineInventoryByMedicineId(int medicineId);
        List<User> GetAllUsers();
        List<Supplier> GetAllSuppliers();
        Medicine? GetMedicine(int medicineId);
        List<MedicineInventory> GetMedicineInventory(int medicineId, bool orderByExpiry = false);
        List<MedicineInventory> GetAvailableMedicineInventory(int medicineId);
        double GetMedicineQuantity(int medicineId);
        DateTime? CalculateExpiryDate(DateTime? manufacturingDate, int? shelfLife);
        bool AddMedicineInventoryList(List<MedicineInventory> inventoryList);
        bool UpdateMedicineInventory(MedicineInventory medicineInventory);
        bool UpdateMedicineInInventory(List<MedicineInventory> medicineInventory);
        List<MedicineInventory> GetNearExpiryMedicines(int monthsThreshold = 6);
        List<Medicine> GetLowStockMedicines(double minimumThreshold);
        List<MedicineInventory> GetExpiredMedicines();
        List<MedicineInventory> GetMedicinesByBatchNumber(string batchNumber);
        List<Medicine> SearchMedicineByName(string name);
        Task<List<MedicineInventory>> SearchMedicinesAsync(
            int? medicineId = null, string? medicineName = null,
            string? activeIngredient = null, string? dosage = null,
            string? dosageForm = null, double? quantity = null,
            double? importPrice = null,
            DateTime? expiryDate = null, string? batchNumber = null,
            string? bidNumber = null, bool? status = null,
            DateTime? minExpiryDate = null, DateTime? maxExpiryDate = null);
        List<MedicineDTO> GetFilteredMedicineInventory(MedicineInventoryFilter filter);
        bool CheckDuplicateBatch(int medicineId, string? batchNumber, DateTime? transactionDate);
        MedicineInventory GetInventoryById(int inventoryId);
        List<MedicineInventory> GetRecentInventoriesByUser(int userId);
        List<MedicineInventory> GetAllInventoriesByUser(int userId);
        int ConsumeMedicineByMedicineId(ConsumeMedicineDTO consumeMedicineDTO);

        Dictionary<Medicine, double> GetAllMedicineConsumeReport(DateTime? from, DateTime? to);
        List<MedicineInventory> GetAllMedicineInventories();
        double? GetActualMedicineQuantity(int medicineId, DateTime date);
        Medicine? GetMedicineByMedicineInventoryId(int id);
        MedicineInventory? GetMedicineInventoryById(int id);
        MedicineConsumption? GetMedicineConsumptionById(int id);
        bool UpdateMedicineConsumption(MedicineConsumption medicineConsumption);
        List<MedicineInventory> GetMedicineImportHistory(DateTime fromDate, DateTime toDate);
        double? GetMedicineQuantityById(int medicineId);
        double GetAddOnMedicineInventory(int id, DateTime? from, DateTime? to);
        double GetNumberOfExpiredMedicineInventory(int medicineInventoryId, DateTime? from, DateTime? to);
        List<MedicineConsumption> MedicineConsumptionDetail(int id, DateTime? from, DateTime? to);
        List<MedicineConsumption> ConsumptionHistory(DateTime? from, DateTime? to);
        bool SaveChanges();
        List<MedicineInventoryStatistic> GetAllMedicineInventoryStatistics();
        List<MedicineInventoryStatistic>? GetMedicineInventoryStatisticsByStatisticDate(DateTime from, DateTime to);
        List<MedicineInventoryStatistic> GetAllMSISNotConfirm();
        bool AddMedicineInventoryStatistic(List<MedicineInventoryStatistic> medicineInventoryStatistic);
        bool UpdateMedicineInventoryStatistic(MedicineInventoryStatistic medicineInventoryStatistic);
        bool UpdateMedicineInventoryStatistic(List<MedicineInventoryStatistic> medicineInventoryStatistics);
        MedicineInventoryStatistic? GetMedicineInventoryStatisticById(int id);
        bool DeleteMedicineInventoryStatistic(MedicineInventoryStatistic medicineInventoryStatistic);
    }

}
