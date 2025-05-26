using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using System.Text.RegularExpressions;

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
                    Note = record.Note,
                    DateCreated = record.DateCreated,
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
                    Note = record.Note,
                    DateCreated = record.DateCreated,
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
                Note = record.Note,
                DateCreated = record.DateCreated,
            };
        }

        public bool AddMedicalRecord(MedicalRecordDTO medicalRecordDTO)
        {
            //  Validate ngày sinh (Dob)
            var dobDate = medicalRecordDTO.Dob.Value.Date;
            var today = DateTime.Now.Date;
            var minDob = today.AddYears(-150);
            if (dobDate > today)
                throw new Exception("Ngày sinh không thể lớn hơn ngày hiện tại!");
            if (dobDate < minDob)
                throw new Exception("Tuổi bệnh nhân không hợp lệ (tối đa 150 tuổi)!");

            //  Validate sdt
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.PhoneNumber) && !Regex.IsMatch(medicalRecordDTO.PhoneNumber, @"^[0-9]{10,11}$"))
                throw new Exception("Số điện thoại phải có 10-11 chữ số!");

            //  Validate email
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.Email) && !Regex.IsMatch(medicalRecordDTO.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                throw new Exception("Email không hợp lệ (vd: email@domain.com)!");

            //  Kiểm tra trùng sdt
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.PhoneNumber))
            {
                var existingRecords = _medicalRecordRepository.GetAllMedicalRecords();
                var isDuplicatePhone = existingRecords.Any(record =>
                    !string.IsNullOrWhiteSpace(record.PhoneNumber) &&
                    record.PhoneNumber.Trim() == medicalRecordDTO.PhoneNumber.Trim());
                if (isDuplicatePhone)
                    throw new Exception("Số điện thoại đã tồn tại!");
            }

            //  Kiểm tra trùng email
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.Email))
            {
                var existingRecords = _medicalRecordRepository.GetAllMedicalRecords();
                var isDuplicateEmail = existingRecords.Any(record =>
                    !string.IsNullOrWhiteSpace(record.Email) &&
                    record.Email.Trim().ToLower() == medicalRecordDTO.Email.Trim().ToLower());
                if (isDuplicateEmail)
                    throw new Exception("Email đã tồn tại!");
            }

            //  Validate mã BHYT
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.HealthInsurance) && !Regex.IsMatch(medicalRecordDTO.HealthInsurance, @"^[A-Za-z]{2}[0-9]{13}$"))
                throw new Exception("Số bảo hiểm y tế phải có 15 ký tự (2 chữ cái đầu + 13 số)!");

            //  Kiểm tra trùng mã BHYT
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.HealthInsurance))
            {
                var existingRecords = _medicalRecordRepository.GetAllMedicalRecords();
                var isDuplicate = existingRecords.Any(record =>
                    !string.IsNullOrWhiteSpace(record.HealthInsurance) &&
                    record.HealthInsurance.Trim().ToLower() == medicalRecordDTO.HealthInsurance.Trim().ToLower());
                if (isDuplicate)
                    throw new Exception("Số bảo hiểm y tế đã tồn tại!");
            }



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
                Note = medicalRecordDTO.Note,
                DateCreated = DateTime.Now,
            };
            if (!_medicalRecordRepository.AddMedicalRecordHistory(record)) return false;
            return true;
        }

        public bool UpdateMedicalRecord(MedicalRecordDTO medicalRecordDTO)
        {
            // Kiểm tra ID bệnh án tồn tại
            var existingRecord = _medicalRecordRepository.GetMedicalRecord(medicalRecordDTO.MedicalRecordId);
            if (existingRecord == null)
                throw new Exception("Bệnh án không tồn tại!");


            //  Validate ngày sinh 
            var dobDate = medicalRecordDTO.Dob.Value.Date;
            var today = DateTime.Now.Date;
            var minDob = today.AddYears(-150);
            if (dobDate > today)
                throw new Exception("Ngày sinh không thể lớn hơn ngày hiện tại!");
            if (dobDate < minDob)
                throw new Exception("Tuổi bệnh nhân không hợp lệ (tối đa 150 tuổi)!");

            //  Validate sdt
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.PhoneNumber) && !Regex.IsMatch(medicalRecordDTO.PhoneNumber, @"^[0-9]{10,11}$"))
                throw new Exception("Số điện thoại phải có 10-11 chữ số!");

            //  Validate email
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.Email) && !Regex.IsMatch(medicalRecordDTO.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                throw new Exception("Email không hợp lệ (vd: email@domain.com)!");

            //  Kiểm tra trùng sdt (trừ bản ghi hiện tại)
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.PhoneNumber))
            {
                var existingRecords = _medicalRecordRepository.GetAllMedicalRecords();
                var isDuplicatePhone = existingRecords.Any(record =>
                    !string.IsNullOrWhiteSpace(record.PhoneNumber) &&
                    record.PhoneNumber.Trim() == medicalRecordDTO.PhoneNumber.Trim() &&
                    record.MedicalRecordId != medicalRecordDTO.MedicalRecordId);
                if (isDuplicatePhone)
                    throw new Exception("Số điện thoại đã tồn tại!");
            }

            //  Kiểm tra trùng email (trừ bản ghi hiện tại)
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.Email))
            {
                var existingRecords = _medicalRecordRepository.GetAllMedicalRecords();
                var isDuplicateEmail = existingRecords.Any(record =>
                    !string.IsNullOrWhiteSpace(record.Email) &&
                    record.Email.Trim().ToLower() == medicalRecordDTO.Email.Trim().ToLower() &&
                    record.MedicalRecordId != medicalRecordDTO.MedicalRecordId);
                if (isDuplicateEmail)
                    throw new Exception("Email đã tồn tại!");
            }

            //  Validate mã BHYT
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.HealthInsurance) && !Regex.IsMatch(medicalRecordDTO.HealthInsurance, @"^[A-Za-z]{2}[0-9]{13}$"))
                throw new Exception("Số bảo hiểm y tế phải có 15 ký tự (2 chữ cái đầu + 13 số)!");

            //  Kiểm tra trùng mã BHYT (trừ bản ghi hiện tại)
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.HealthInsurance))
            {
                var existingRecords = _medicalRecordRepository.GetAllMedicalRecords();
                var isDuplicate = existingRecords.Any(record =>
                    !string.IsNullOrWhiteSpace(record.HealthInsurance) &&
                    record.HealthInsurance.Trim().ToLower() == medicalRecordDTO.HealthInsurance.Trim().ToLower() &&
                    record.MedicalRecordId != medicalRecordDTO.MedicalRecordId);
                if (isDuplicate)
                    throw new Exception("Số bảo hiểm y tế đã tồn tại!");
            }


            // Gán các trường có thể được cập nhật 
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
