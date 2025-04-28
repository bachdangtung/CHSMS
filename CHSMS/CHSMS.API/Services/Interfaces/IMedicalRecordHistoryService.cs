using CHSMS.API.DTOs.MedicalRecord;

namespace CHSMS.API.Services.Interfaces
{
    public interface IMedicalRecordHistoryService
    {
        List<MedicalRecordHistoryDTO> GetAllMedicalRecordHistories();
        MedicalRecordHistoryDTO? GetMedicalRecordHistory(int medicalRecordHistoryId);
        List<MedicalRecordHistoryDTO> GetMedicalRecordHistoryByPatientId(int medicalRecordId, DateTime? startDate, DateTime? endDate, string? doctorName);
        List<MedicalRecordHistoryDTO> GetMedicalRecordHistoriesByFilter(string? doctorName, string? patientName);
        bool AddMedicalRecordHistory(int userId, MedicalRecordHistoryDTO medicalRecordDTO);
        bool UpdateMedicalRecordHistory(MedicalRecordHistoryDTO medicalRecordDTO);
        /*        bool DeleteMedicalRecordHistory(int medicalRecordId);
        */
        int GetTodayMedicalRecordHistoryCount();
        List<UserDTO> GetAllUsers();
    }
}
