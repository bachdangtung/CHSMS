using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;

namespace CHSMS.API.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository)
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

        public List<MedicalRecordDTO> GetAllMedicalRecordsByPatientName(string? patientName, string? healthInsurance)
        {
            List<MedicalRecordDTO> medicalRecordDTOs = new List<MedicalRecordDTO>();
            foreach (var record in _medicalRecordRepository.GetMedicalRecordsByPatientName(patientName, healthInsurance))
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

        public MedicalRecordDTO? GetMedicalRecord(int medicalRecordId)
        {
            var record = _medicalRecordRepository.GetMedicalRecord(medicalRecordId);
            if (record == null) return null;

            return new MedicalRecordDTO
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
        }

        public bool AddMedicalRecord(MedicalRecordDTO medicalRecordDTO)
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

        public bool UpdateMedicalRecord(MedicalRecordDTO medicalRecordDTO)
        {
            var existingRecord = _medicalRecordRepository.GetMedicalRecord(medicalRecordDTO.MedicalRecordId);
            if (existingRecord == null) return false;

            // Gán các trường có thể được cập nhật (nếu muốn, có thể check null để chỉ update khi có giá trị)
            existingRecord.PatientName = medicalRecordDTO.PatientName;
            existingRecord.Gender = medicalRecordDTO.Gender;
            existingRecord.Dob = medicalRecordDTO.Dob;
            existingRecord.EthnicGroup = medicalRecordDTO.EthnicGroup;
            existingRecord.EducationLevel = medicalRecordDTO.EducationLevel;
            existingRecord.HealthInsurance = medicalRecordDTO.HealthInsurance;
            existingRecord.Address = medicalRecordDTO.Address;
            existingRecord.PhoneNumber = medicalRecordDTO.PhoneNumber;
            existingRecord.Email = medicalRecordDTO.Email;
            existingRecord.Job = medicalRecordDTO.Job;
            existingRecord.Status = medicalRecordDTO.Status;
            existingRecord.Note = medicalRecordDTO.Note;

            return _medicalRecordRepository.UpdateMedicalRecord(existingRecord);
        }
    }
}
