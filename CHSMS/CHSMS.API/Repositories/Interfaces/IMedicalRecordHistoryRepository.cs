using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IMedicalRecordHistoryRepository
    {
        List<MedicalRecordHistory> GetAllMedicalRecordHistories();
        List<MedicalRecordHistory> GetAllTodayMedicalRecordHistories();
        int CountTodayMedicalRecordHistories();
        MedicalRecordHistory? GetMedicalRecordHistory(int medicalRecordHistoryId);
        List<MedicalRecordHistory> GetMedicalRecordHistoryByPatientId(int medicalRecordId, DateTime? startDate, DateTime? endDate, string? doctorName);
        List<MedicalRecordHistory> GetMedicalRecordHistoriesByFilter(string? doctorName, string? patientName);
        bool AddMedicalRecordHistory(MedicalRecordHistory medicalRecordHistory);
        bool UpdateMedicalRecordHistory(MedicalRecordHistory medicalRecordHistory);
        /*        bool DeleteMedicalRecordHistory(int medicalRecordId);
        */
        List<User> GetAllUsers();
    }
}
