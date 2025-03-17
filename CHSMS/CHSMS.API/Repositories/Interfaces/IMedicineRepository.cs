using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IMedicineRepository
    {
        List<Medicine> GetAllMedicine();
        Medicine? GetMedicine(int medicine);
        List<MedicineInventory> GetMedicineDetail(int medicineId);
        List<MedicineInventory> GetAvailableMedicineInventory(int medicineId);
        double GetMedicineQuantity(int medicineId);
        bool AddMedicineInventory(MedicineInventory medicineInventory);
        bool UpdateMedicineInventory(MedicineInventory medicineInventory);
        DateTime? CalculateExpiryDate(DateTime? manufacturingDate, int? shelfLife);
        Task<dynamic> SearchMedicinesData(string query);
    }
}
