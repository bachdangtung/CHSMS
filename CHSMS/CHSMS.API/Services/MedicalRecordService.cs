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
            // 1. Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.PatientName))
                throw new Exception("Tên bệnh nhân không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Gender))
                throw new Exception("Giới tính không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Address))
                throw new Exception("Địa chỉ không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.EthnicGroup))
                throw new Exception("Dân tộc không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.EducationLevel))
                throw new Exception("Trình độ học vấn không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Job))
                throw new Exception("Nghề nghiệp không được để trống!");

            // 2. Validate ngày sinh (Dob)
            if (!medicalRecordDTO.Dob.HasValue)
                throw new Exception("Ngày sinh không được để trống!");
            var dobDate = medicalRecordDTO.Dob.Value.Date;
            var today = DateTime.Now.Date;
            var minDob = today.AddYears(-150);
            if (dobDate > today)
                throw new Exception("Ngày sinh không thể lớn hơn ngày hiện tại!");
            if (dobDate < minDob)
                throw new Exception("Tuổi bệnh nhân không hợp lệ (tối đa 150 tuổi)!");

            // 3. Validate số điện thoại
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.PhoneNumber) && !Regex.IsMatch(medicalRecordDTO.PhoneNumber, @"^[0-9]{10,11}$"))
                throw new Exception("Số điện thoại phải có 10-11 chữ số!");

            // 4. Validate email
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.Email) && !Regex.IsMatch(medicalRecordDTO.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                throw new Exception("Email không hợp lệ (vd: email@domain.com)!");

            // 5. Validate mã BHYT
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.HealthInsurance) && !Regex.IsMatch(medicalRecordDTO.HealthInsurance, @"^[A-Za-z]{2}[0-9]{13}$"))
                throw new Exception("Số bảo hiểm y tế phải có 15 ký tự (2 chữ cái đầu + 13 số)!");

            // 6. Kiểm tra trùng mã BHYT
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
                Note = medicalRecordDTO.Note
            };
            if (!_medicalRecordRepository.AddMedicalRecordHistory(record)) return false;
            return true;
        }

        public bool UpdateMedicalRecord(MedicalRecordDTO medicalRecordDTO)
        {
            // 1. Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.PatientName))
                throw new Exception("Tên bệnh nhân không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Gender))
                throw new Exception("Giới tính không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Address))
                throw new Exception("Địa chỉ không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.EthnicGroup))
                throw new Exception("Dân tộc không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.EducationLevel))
                throw new Exception("Trình độ học vấn không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Job))
                throw new Exception("Nghề nghiệp không được để trống!");

            // 2. Validate ngày sinh (Dob)
            if (!medicalRecordDTO.Dob.HasValue)
                throw new Exception("Ngày sinh không được để trống!");
            var dobDate = medicalRecordDTO.Dob.Value.Date;
            var today = DateTime.Now.Date;
            var minDob = today.AddYears(-150);
            if (dobDate > today)
                throw new Exception("Ngày sinh không thể lớn hơn ngày hiện tại!");
            if (dobDate < minDob)
                throw new Exception("Tuổi bệnh nhân không hợp lệ (tối đa 150 tuổi)!");

            // 3. Validate số điện thoại
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.PhoneNumber) && !Regex.IsMatch(medicalRecordDTO.PhoneNumber, @"^[0-9]{10,11}$"))
                throw new Exception("Số điện thoại phải có 10-11 chữ số!");

            // 4. Validate email
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.Email) && !Regex.IsMatch(medicalRecordDTO.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                throw new Exception("Email không hợp lệ (vd: email@domain.com)!");

            // 5. Validate mã BHYT
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.HealthInsurance) && !Regex.IsMatch(medicalRecordDTO.HealthInsurance, @"^[A-Za-z]{2}[0-9]{13}$"))
                throw new Exception("Số bảo hiểm y tế phải có 15 ký tự (2 chữ cái đầu + 13 số)!");

            // 6. Kiểm tra trùng mã BHYT (trừ bản ghi hiện tại)
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
