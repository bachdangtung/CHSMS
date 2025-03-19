using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using CHSMS.API.Repositories.Interfaces;

namespace CHSMS.API.Services
{
    public class MedicalRecordHistoryService
    {
        private readonly MedicalRecordHistoryRepository _medicalRecordHistoryRepository;
        private readonly IUserRepository _userRepository;

        public MedicalRecordHistoryService(MedicalRecordHistoryRepository medicalRecordHistoryRepository, IUserRepository userRepository)
        {
            _medicalRecordHistoryRepository = medicalRecordHistoryRepository;
            _userRepository = userRepository;
        }


        public List<MedicalRecordHistoryDTO> GetAllMedicalRecordHistories()
        {
            List<MedicalRecordHistoryDTO> medicalRecordDTOs = new List<MedicalRecordHistoryDTO>();
            foreach (var record in _medicalRecordHistoryRepository.GetAllMedicalRecordHistories())
            {
                var recordDTO = new MedicalRecordHistoryDTO
                {
                    MedicalRecordHistoryId = record.MedicalRecordHistoryId,
                    PatientId = record.MedicalRecordId,
                    UserId = record.UserId,
                    DoctorName = record.User?.UserName,
                    PatientName = record.MedicalRecord?.PatientName,
                    Gender = record.MedicalRecord?.Gender,
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


        public MedicalRecordHistoryDTO? GetMedicalRecordHistory(int medicalRecordId)
        {
            var record = _medicalRecordHistoryRepository.GetMedicalRecordHistory(medicalRecordId);
            if (record == null) return null;

            return new MedicalRecordHistoryDTO
            {
                MedicalRecordHistoryId = record.MedicalRecordHistoryId,
                PatientId = record.MedicalRecordId,
                UserId = record.UserId,
                DoctorName = record.User?.UserName,
                PatientName = record.MedicalRecord?.PatientName,
                Gender = record.MedicalRecord?.Gender,
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
        }

        public List<MedicalRecordHistoryDTO> GetMedicalRecordHistoriesByFilter(DateTime? startDate, DateTime? endDate, string? doctorName, string? patientName)
        {
            List<MedicalRecordHistoryDTO> medicalRecordDTOs = new List<MedicalRecordHistoryDTO>();
            var records = _medicalRecordHistoryRepository.GetMedicalRecordHistoriesByFilter(startDate, endDate, doctorName, patientName);
            foreach (var record in records)
            {
                medicalRecordDTOs.Add(new MedicalRecordHistoryDTO
                {
                    MedicalRecordHistoryId = record.MedicalRecordHistoryId,
                    PatientId = record.MedicalRecordId,
                    UserId = record.UserId,
                    DoctorName = record.User?.UserName,
                    PatientName = record.MedicalRecord?.PatientName,
                    Dob = record.MedicalRecord?.Dob,
                    Gender = record.MedicalRecord?.Gender,
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
                });
            }
            return medicalRecordDTOs;
        }


        public bool AddMedicalRecordHistory(MedicalRecordHistoryDTO medicalRecordDTO)
        {
            var record = new MedicalRecordHistory
            {
                 MedicalRecordHistoryId = 0,
                MedicalRecordId = medicalRecordDTO.PatientId,
                Diagnose = medicalRecordDTO.Diagnosis,
                TreatmentMethod = medicalRecordDTO.TreatmentMethod,
                Symptom = medicalRecordDTO.Symptom,
                Date = medicalRecordDTO.RecordDate,
                Pulse = medicalRecordDTO.Pulse,
                BloodPressure = medicalRecordDTO.BloodPressure,
                RespiratoryRate = medicalRecordDTO.RespiratoryRate,
                Temperature = medicalRecordDTO.Temperature,
                Height = medicalRecordDTO.Height,
                Weight = medicalRecordDTO.Weight,
                Note = medicalRecordDTO.Note
            };
            if (!_medicalRecordHistoryRepository.AddMedicalRecordHistory(record)) return false;
            return true;
        }


        public bool UpdateMedicalRecordHistory(MedicalRecordHistoryDTO medicalRecordDTO)
        {
            var record = new MedicalRecordHistory
            {
                MedicalRecordHistoryId = medicalRecordDTO.MedicalRecordHistoryId,
                UserId = medicalRecordDTO.UserId,
                MedicalRecordId = medicalRecordDTO.PatientId,
                Diagnose = medicalRecordDTO.Diagnosis,
                TreatmentMethod = medicalRecordDTO.TreatmentMethod,
                Symptom = medicalRecordDTO.Symptom,
                Date = medicalRecordDTO.RecordDate,
                Pulse = medicalRecordDTO.Pulse,
                BloodPressure = medicalRecordDTO.BloodPressure,
                RespiratoryRate = medicalRecordDTO.RespiratoryRate,
                Temperature = medicalRecordDTO.Temperature,
                Height = medicalRecordDTO.Height,
                Weight = medicalRecordDTO.Weight,
                Note = medicalRecordDTO.Note
            };
            return _medicalRecordHistoryRepository.UpdateMedicalRecordHistory(record);
        }


        public bool DeleteMedicalRecordHistory(int medicalRecordId)
        {
            return _medicalRecordHistoryRepository.DeleteMedicalRecordHistory(medicalRecordId);
        }

        public List<UserDTO> GetAllUsers()
        {
            List<UserDTO> medicalRecordDTOs = new List<UserDTO>();
            foreach (var record in _medicalRecordHistoryRepository.GetAllUsers())
            {
                var recordDTO = new UserDTO
                {
                    UserId = record.UserId,
                    UserName = record.UserName,
                    Gender = record.Gender
                };
                medicalRecordDTOs.Add(recordDTO);
            }
            return medicalRecordDTOs;
        }


    }
}
