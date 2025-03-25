using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
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

        public List<MedicalRecordDTO> GetAllMedicalRecordsByPatientName(string? patientName)
        {
            List<MedicalRecordDTO> medicalRecordDTOs = new List<MedicalRecordDTO>();
            foreach (var record in _medicalRecordRepository.GetMedicalRecordsByPatientName(patientName))
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

        public bool AddMedicalRecordHistory(MedicalRecordDTO medicalRecordDTO)
        {
            var record = new MedicalRecord
            {
                MedicalRecordId = 0,
                PatientName = medicalRecordDTO.PatientName,
                Gender = medicalRecordDTO.Gender,
                Dob = medicalRecordDTO.Dob,
                EthnicGroup = medicalRecordDTO.EthnicGroup,
                EducationLevel = medicalRecordDTO.EducationLevel,
                HealthInsurance = medicalRecordDTO.HealthInsurance,
                Address = medicalRecordDTO.Address,
                PhoneNumber = medicalRecordDTO.PhoneNumber,
                Email = medicalRecordDTO.Email,
                Job = medicalRecordDTO.Job,
                Status = medicalRecordDTO.Status,
                Note = medicalRecordDTO.Note
            };
            if (!_medicalRecordRepository.AddMedicalRecordHistory(record)) return false;
            return true;
        }
    }
}
