using CHSMS.API.DTOs.ExternalPrescription;
using CHSMS.API.Models;

namespace CHSMS.API.Services.Interfaces
{
    public interface IExternalPrescriptionService
    {
        Task<int> CreateExternalPrescriptionAsync(int userId, int medicalRecordHistoryId, CreateExternalPrescriptionDTO dto);
        Task<List<Medicine>> GetMedicinesForExternalPrescriptionAsync();
        Task EditExternalPrescriptionForDoctorAsync(EditExternalPrescriptionDTO dto);
        Task<List<ExternalPrescriptionDTO>> GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId);
        Task<ExternalPrescriptionDetailDTO> GetExternalPrescriptionDetailAsync(int externalPrescriptionId);
    }
}
