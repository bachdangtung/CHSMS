using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> CreatePrescriptionAsync(Prescription prescription);
        Task<MedicineInventory> GetMedicineInventoryByIdAsync(int medicineInventoryId);
        Task<MedicineConsumption> CreateMedicineConsumptionAsync(MedicineConsumption consumption);
        Task<PrescriptionMedicineConsumption> CreatePrescriptionMedicineConsumptionAsync(PrescriptionMedicineConsumption pmc);
        Task<List<MedicineInventory>> GetAvailableMedicinesAsync();
        Task UpdateMedicineInventoryAsync(MedicineInventory inventory);
        Task<List<Prescription>> GetPrescriptionsByUserIdAsync(int userId);
        Task<List<Prescription>> GetAllPrescriptionsAsync();
        Task<List<Prescription>> GetAllPrescriptionsNoBHYTAsync();
        Task<List<Prescription>> GetPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId);
        Task<Prescription> GetPrescriptionDetailAsync(int prescriptionId);
        Task<Prescription> GetPrescriptionByIdAsync(int prescriptionId);
        Task<List<MedicineConsumption>> GetMedicineConsumptionsByPrescriptionIdAsync(int prescriptionId);
        Task UpdatePrescriptionAsync(Prescription prescription);
        Task<PrescriptionMedicineConsumption> GetPrescriptionMedicineConsumptionByConsumptionIdAsync(int medicineConsumptionId);
        Task DeletePrescriptionMedicineConsumptionAsync(int prescriptionId, int medicineConsumptionId);
        Task DeleteMedicineConsumptionAsync(int consumptionId);
        Task<MedicineConsumption> GetMedicineConsumptionByIdAsync(int medicineConsumptionId);
        Task UpdateMedicineConsumptionAsync(MedicineConsumption consumption);
        Task UpdatePrescriptionMedicineConsumptionAsync(PrescriptionMedicineConsumption pmc);
        int CountTodayPrescriptions();
        Task<List<PrescriptionMedicineConsumption>> GetAllMedicineConsumptionsAsync();
    }
}
