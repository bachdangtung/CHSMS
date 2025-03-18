using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Repositories;

namespace CHSMS.API.Services
{
    public class MedicalRecordService
    {
        private readonly MedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordService(MedicalRecordRepository medicalRecordRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
        }
        public List<MedicalRecordDTO> GetAllMedicalRecords()
        {
            List<MedicalRecordDTO> medicalRecordDTOs = new List<MedicalRecordDTO>();
            foreach (var record in _medicalRecordRepository.GetAllMedicalRecords())
            {
                var recordDTO = new MedicalRecordDTO
                {
                    MedicalRecordId = record.MedicalRecordId,
                    PatientName = record.PatientName,
                    Gender = record.Gender,
                    Dob = record.Dob,
                    EducationLevel = record.EducationLevel,
                    HealthInsurance = record.HealthInsurance,
                    Dob = record.MedicalRecord?.Dob,
                    HealthInsurance = record.MedicalRecord?.HealthInsurance,
                    Address = record.MedicalRecord?.Address,
                    Job = record.MedicalRecord?.Job,
                    EthnicGroup = record.MedicalRecord?.EthnicGroup,
                    UserName = record.User?.UserName,
                    Diagnosis = record.Diagnose,
                    TreatmentMethod = record.TreatmentMethod,
                    Symptom = record.Symptom,
                    RecordDate = record.Date,
                    Note = record.Note
                };
                medicalRecordDTOs.Add(recordDTO);
            }
            return medicalRecordDTOs;
        }
    }
}
