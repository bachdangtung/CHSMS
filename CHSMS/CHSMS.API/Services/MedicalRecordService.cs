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
                    EthnicGroup = record.EthnicGroup,
                    EducationLevel = record.EducationLevel,
                    HealthInsurance = record.HealthInsurance,
                    Address = record.Address,                    
                    PhoneNumber = record.PhoneNumber,
                    Email = record.Email,
                    Job = record.Job,
                    Status = record.Status,
                    Note = record.Note
                };
                medicalRecordDTOs.Add(recordDTO);
            }
            return medicalRecordDTOs;
        }
    }
}
