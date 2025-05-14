using CHSMS.API.DTOs.MedicalRecord;

namespace CHSMS.API.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        List<MedicalRecordDTO> GetAllMedicalRecords();
        List<MedicalRecordDTO> GetAllMedicalRecordsByPatientName(string? patientName, string? healthInsurance);
        MedicalRecordDTO? GetMedicalRecord(int medicalRecordId);
        bool AddMedicalRecord(MedicalRecordDTO medicalRecordDTO);
        bool UpdateMedicalRecord(MedicalRecordDTO medicalRecordDTO);
    }
}
