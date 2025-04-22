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
        List<MedicineInventory> GetNearExpiryMedicines(int monthsThreshold = 6);
        List<Medicine> GetLowStockMedicines(double minimumThreshold);
        List<MedicineInventory> GetExpiredMedicines();
        List<MedicineInventory> GetMedicinesByBatchNumber(string batchNumber);
        List<Medicine> SearchMedicineByName(string name);
        Task<List<Medicine>> SearchMedicinesAsync(
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
        bool SaveChanges();
    }

}
