using CHSMS.API.DTOs.MedicalRecord;

namespace CHSMS.API.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        List<MedicalRecordDTO> GetAllMedicalRecords();
        List<MedicalRecordDTO> GetAllMedicalRecordsByPatientName(string? patientName, string? healthInsurance);
        bool AddMedicalRecordHistory(MedicalRecordDTO medicalRecordDTO);
    }
}
