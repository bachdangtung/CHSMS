using CHSMS.API.DTOs;
using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.MedicineInventory;
using CHSMS.API.DTOs.Prescription;

namespace CHSMS.API.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<int> CreatePrescriptionAsync(int userId, int medicalRecordHistoryId, CreatePrescriptionDTO dto);
        Task EditPrescriptionForDoctorAsync(EditPrescriptionForDoctorDTO dto);
        Task EditPrescriptionForPharmacistAsync(EditPrescriptionForPharmacistDTO dto);
        Task<List<MedicineInventoryDTO>> GetAllMedicinesInInventoryAsync();
        Task<List<PrescriptionDTO>> GetPrescriptionsByUserIdListAsync(int userId);
        Task<List<PrescriptionDTO>> GetAllPrescriptionsAsync();
        Task<List<PrescriptionDTO>> GetTodayPrescriptionsAsync();
        Task<List<PrescriptionDTO>> GetAllPrescriptionsNoBHYTAsync();
        Task<List<PrescriptionDTO>> GetTodayPrescriptionsNoBHYTAsync();
        Task<List<PrescriptionDTO>> GetPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId);
        Task<PrescriptionDetailDTO> GetPrescriptionDetailAsync(int prescriptionId);
        int GetTodayPrescriptionCount();
        Task<List<MedicineConsumptionStatisticDTO>> GetAllMedicineConsumptionsAsync();
    }
}
