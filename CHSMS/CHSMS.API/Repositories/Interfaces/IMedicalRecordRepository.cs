using CHSMS.API.Models;

namespace CHSMS.API.Repositories.Interfaces
{
    public interface IMedicalRecordRepository
    {
        List<MedicalRecord> GetAllMedicalRecords();
        List<MedicalRecord> GetMedicalRecordsByPatientName(string? patientName, string? healthInsurance);
        MedicalRecord? GetMedicalRecord(int medicalRecordId);
        bool AddMedicalRecordHistory(MedicalRecord medicalRecord);
        bool UpdateMedicalRecord(MedicalRecord medicalRecord);
    }
}
