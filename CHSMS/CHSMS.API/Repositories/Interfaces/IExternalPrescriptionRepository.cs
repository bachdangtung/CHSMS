using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IExternalPrescriptionRepository
    {
        Task<ExternalPrescription> CreateExternalPrescriptionAsync(ExternalPrescription externalPrescription);
        Task CreateExternalMedicinePrescriptionAsync(MedicinePrescription medicinePrescription);
        Task<List<Medicine>> GetMedicinesForExternalPrescriptionAsync();
        Task<ExternalPrescription> GetExternalPrescriptionByIdAsync(int prescriptionId);
        Task UpdateExternalPrescriptionAsync(ExternalPrescription prescription);
        Task<List<MedicinePrescription>> GetMedicinePrescriptionsByPrescriptionIdAsync(int prescriptionId);
        Task DeleteMedicinePrescriptionAsync(int prescriptionId, int medicineId);
        Task<List<ExternalPrescription>> GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId);
        Task<ExternalPrescription?> GetExternalPrescriptionDetailAsync(int externalPrescriptionId);
    }
}
