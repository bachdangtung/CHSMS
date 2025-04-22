using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IMedicalRecordRepository
    {
        List<MedicalRecord> GetAllMedicalRecords();
        List<MedicalRecord> GetMedicalRecordsByPatientName(string? patientName);
        bool AddMedicalRecordHistory(MedicalRecord medicalRecord);
    }
}
