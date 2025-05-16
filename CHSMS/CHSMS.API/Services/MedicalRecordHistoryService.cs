using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace CHSMS.API.Services
{
    public class MedicalRecordHistoryService : IMedicalRecordHistoryService
    {
        private readonly IMedicalRecordHistoryRepository _medicalRecordHistoryRepository;
        private readonly IUserRepository _userRepository;

        public MedicalRecordHistoryService(IMedicalRecordHistoryRepository medicalRecordHistoryRepository, IUserRepository userRepository)
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
                    Fullname = record.User?.Fullname,
                    Gender = record.MedicalRecord?.Gender,
                    Dob = record.MedicalRecord?.Dob,
                    HealthInsurance = record.MedicalRecord?.HealthInsurance,
                    Address = record.MedicalRecord?.Address,
                    Job = record.MedicalRecord?.Job,
                    EthnicGroup = record.MedicalRecord?.EthnicGroup,
                    UserName = record.User?.UserName,
                    DiagnoseConclusion = record.DiagnoseConclusion,
                    TreatmentMethod = record.TreatmentMethod,
                    Symptom = record.Symptom,
                    RecordDate = record.Date,
                    Note = record.Note,
                    BloodPressure = record.BloodPressure,
                    Pulse = record.Pulse,
                    RespiratoryRate = record.RespiratoryRate,
                    Temperature = record.Temperature,
                    Height = record.Height,
                    Weight = record.Weight,
                    MedicalRecordHistoryCode = record.MedicalRecordHistoryCode,
                    PatientCategory = record.PatientCategory,
                    DiseaseProgress = record.DiseaseProgress,
                    DiseaseStage = record.DiseaseStage,
                    ICD = record.Icd,
                    MedicalOrder = record.MedicalOrder,
                    TreatmentBed = record.TreatmentBed
                };
                medicalRecordDTOs.Add(recordDTO);
            }
            return medicalRecordDTOs;
        }



        public MedicalRecordHistoryDTO? GetMedicalRecordHistory(int medicalRecordHistoryId)
        {
            var record = _medicalRecordHistoryRepository.GetMedicalRecordHistory(medicalRecordHistoryId);
            if (record == null) return null;

            return new MedicalRecordHistoryDTO
            {
                MedicalRecordHistoryId = record.MedicalRecordHistoryId,
                PatientId = record.MedicalRecordId,
                UserId = record.UserId,
                Fullname = record.User?.Fullname,
                DoctorName = record.User?.UserName,
                PatientName = record.MedicalRecord?.PatientName,
                Gender = record.MedicalRecord?.Gender,
                Dob = record.MedicalRecord?.Dob,
                HealthInsurance = record.MedicalRecord?.HealthInsurance,
                Address = record.MedicalRecord?.Address,
                Job = record.MedicalRecord?.Job,
                EthnicGroup = record.MedicalRecord?.EthnicGroup,
                UserName = record.User?.UserName,
                DiagnoseConclusion = record.DiagnoseConclusion,
                TreatmentMethod = record.TreatmentMethod,
                Symptom = record.Symptom,
                RecordDate = record.Date,
                Note = record.Note,
                BloodPressure = record.BloodPressure,
                Pulse = record.Pulse,
                RespiratoryRate = record.RespiratoryRate,
                Temperature = record.Temperature,
                Height = record.Height,
                Weight = record.Weight,
                MedicalRecordHistoryCode = record.MedicalRecordHistoryCode,
                PatientCategory = record.PatientCategory,
                DiseaseProgress = record.DiseaseProgress,
                DiseaseStage = record.DiseaseStage,
                ICD = record.Icd,
                MedicalOrder = record.MedicalOrder,
                TreatmentBed = record.TreatmentBed
            };
        }

        public List<MedicalRecordHistoryDTO> GetMedicalRecordHistoryByPatientId(int medicalRecordId, DateTime? startDate, DateTime? endDate, string? doctorName)
        {
            List<MedicalRecordHistoryDTO> medicalRecordDTOs = new List<MedicalRecordHistoryDTO>();
            foreach (var record in _medicalRecordHistoryRepository.GetMedicalRecordHistoryByPatientId(medicalRecordId, startDate, endDate, doctorName))
            {
                var recordDTO = new MedicalRecordHistoryDTO
                {
                    MedicalRecordHistoryId = record.MedicalRecordHistoryId,
                    PatientId = record.MedicalRecordId,
                    UserId = record.UserId,
                    Fullname = record.User?.Fullname,
                    DoctorName = record.User?.UserName,
                    PatientName = record.MedicalRecord?.PatientName,
                    Gender = record.MedicalRecord?.Gender,
                    Dob = record.MedicalRecord?.Dob,
                    HealthInsurance = record.MedicalRecord?.HealthInsurance,
                    Address = record.MedicalRecord?.Address,
                    Job = record.MedicalRecord?.Job,
                    EthnicGroup = record.MedicalRecord?.EthnicGroup,
                    UserName = record.User?.UserName,
                    DiagnoseConclusion = record.DiagnoseConclusion,
                    TreatmentMethod = record.TreatmentMethod,
                    Symptom = record.Symptom,
                    RecordDate = record.Date,
                    Note = record.Note,
                    BloodPressure = record.BloodPressure,
                    Pulse = record.Pulse,
                    RespiratoryRate = record.RespiratoryRate,
                    Temperature = record.Temperature,
                    Height = record.Height,
                    Weight = record.Weight,
                    MedicalRecordHistoryCode = record.MedicalRecordHistoryCode,
                    PatientCategory = record.PatientCategory,
                    DiseaseProgress = record.DiseaseProgress,
                    DiseaseStage = record.DiseaseStage,
                    ICD = record.Icd,
                    MedicalOrder = record.MedicalOrder,
                    TreatmentBed = record.TreatmentBed
                };
                medicalRecordDTOs.Add(recordDTO);
            }
            return medicalRecordDTOs;
        }

        public List<MedicalRecordHistoryDTO> GetMedicalRecordHistoriesByFilter(string? doctorName, string? patientName)
        {
            List<MedicalRecordHistoryDTO> medicalRecordDTOs = new List<MedicalRecordHistoryDTO>();
            var records = _medicalRecordHistoryRepository.GetMedicalRecordHistoriesByFilter(doctorName, patientName);
            foreach (var record in records)
            {
                medicalRecordDTOs.Add(new MedicalRecordHistoryDTO
                {
                    MedicalRecordHistoryId = record.MedicalRecordHistoryId,
                    PatientId = record.MedicalRecordId,
                    UserId = record.UserId,
                    Fullname = record.User?.Fullname,
                    DoctorName = record.User?.UserName,
                    PatientName = record.MedicalRecord?.PatientName,
                    Dob = record.MedicalRecord?.Dob,
                    Gender = record.MedicalRecord?.Gender,
                    HealthInsurance = record.MedicalRecord?.HealthInsurance,
                    Address = record.MedicalRecord?.Address,
                    Job = record.MedicalRecord?.Job,
                    EthnicGroup = record.MedicalRecord?.EthnicGroup,
                    UserName = record.User?.UserName,
                    DiagnoseConclusion = record.DiagnoseConclusion,
                    TreatmentMethod = record.TreatmentMethod,
                    Symptom = record.Symptom,
                    RecordDate = record.Date,
                    Note = record.Note,
                    BloodPressure = record.BloodPressure,
                    Pulse = record.Pulse,
                    RespiratoryRate = record.RespiratoryRate,
                    Temperature = record.Temperature,
                    Height = record.Height,
                    Weight = record.Weight,
                    MedicalRecordHistoryCode = record.MedicalRecordHistoryCode,
                    PatientCategory = record.PatientCategory,
                    DiseaseProgress = record.DiseaseProgress,
                    DiseaseStage = record.DiseaseStage,
                    ICD = record.Icd,
                    MedicalOrder = record.MedicalOrder,
                    TreatmentBed = record.TreatmentBed
                });
            }
            return medicalRecordDTOs;
        }


        public bool AddMedicalRecordHistory(int userId, MedicalRecordHistoryDTO medicalRecordDTO)
        {
            // 1. Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.MedicalRecordHistoryCode))
                throw new Exception("Mã bệnh án không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.PatientCategory))
                throw new Exception("Đối tượng không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.DiagnoseConclusion))
                throw new Exception("Kết luận chẩn đoán không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.TreatmentMethod))
                throw new Exception("Phương pháp điều trị không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.DiseaseProgress))
                throw new Exception("Diễn biến bệnh không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.MedicalOrder))
                throw new Exception("Y lệnh không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Symptom))
                throw new Exception("Triệu chứng không được để trống!");
            if (!medicalRecordDTO.Pulse.HasValue)
                throw new Exception("Mạch không được để trống!");
            if (!medicalRecordDTO.RespiratoryRate.HasValue)
                throw new Exception("Nhịp thở không được để trống!");
            if (!medicalRecordDTO.Temperature.HasValue)
                throw new Exception("Nhiệt độ không được để trống!");
            if (!medicalRecordDTO.Height.HasValue)
                throw new Exception("Chiều cao không được để trống!");
            if (!medicalRecordDTO.Weight.HasValue)
                throw new Exception("Cân nặng không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.BloodPressure))
                throw new Exception("Huyết áp không được để trống!");

            // 2. Validate ngưỡng sinh lý
            if (medicalRecordDTO.Pulse.HasValue && (medicalRecordDTO.Pulse < 30 || medicalRecordDTO.Pulse > 200))
                throw new Exception("Mạch phải nằm trong khoảng 30 bpm đến 200 bpm!");
            if (medicalRecordDTO.RespiratoryRate.HasValue && (medicalRecordDTO.RespiratoryRate < 10 || medicalRecordDTO.RespiratoryRate > 60))
                throw new Exception("Nhịp thở phải nằm trong khoảng 10 lần/phút đến 60 lần/phút!");
            if (medicalRecordDTO.Temperature.HasValue && (medicalRecordDTO.Temperature < 33 || medicalRecordDTO.Temperature > 45))
                throw new Exception("Nhiệt độ phải nằm trong khoảng 33°C đến 45°C!");
            if (medicalRecordDTO.Height.HasValue && (medicalRecordDTO.Height < 30 || medicalRecordDTO.Height > 250))
                throw new Exception("Chiều cao phải nằm trong khoảng 30 cm đến 250 cm!");
            if (medicalRecordDTO.Weight.HasValue && (medicalRecordDTO.Weight < 1 || medicalRecordDTO.Weight > 300))
                throw new Exception("Cân nặng phải nằm trong khoảng 1 kg đến 300 kg!");

            // 3. Validate huyết áp
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.BloodPressure) && !Regex.IsMatch(medicalRecordDTO.BloodPressure, @"^\d{1,3}/\d{1,3}$"))
                throw new Exception("Huyết áp phải có định dạng 'số/số' (ví dụ: 120/80)!");

            var record = new MedicalRecordHistory
            {
                MedicalRecordHistoryId = 0,
                MedicalRecordId = medicalRecordDTO.PatientId,
                UserId = userId,
                Address = medicalRecordDTO.Address,
                DiagnoseConclusion = medicalRecordDTO.DiagnoseConclusion,
                TreatmentMethod = medicalRecordDTO.TreatmentMethod,
                Symptom = medicalRecordDTO.Symptom,
                Date = medicalRecordDTO.RecordDate,
                Pulse = medicalRecordDTO.Pulse,
                BloodPressure = medicalRecordDTO.BloodPressure,
                RespiratoryRate = medicalRecordDTO.RespiratoryRate,
                Temperature = medicalRecordDTO.Temperature,
                Height = medicalRecordDTO.Height,
                Weight = medicalRecordDTO.Weight,
                Note = medicalRecordDTO.Note,
                MedicalRecordHistoryCode = medicalRecordDTO.MedicalRecordHistoryCode,
                PatientCategory = medicalRecordDTO.PatientCategory,
                DiseaseProgress = medicalRecordDTO.DiseaseProgress,
                DiseaseStage = medicalRecordDTO.DiseaseStage,
                Icd = medicalRecordDTO.ICD,
                MedicalOrder = medicalRecordDTO.MedicalOrder,
                TreatmentBed = medicalRecordDTO.TreatmentBed
            };
            if (!_medicalRecordHistoryRepository.AddMedicalRecordHistory(record)) return false;
            return true;
        }


        public bool UpdateMedicalRecordHistory(MedicalRecordHistoryDTO medicalRecordDTO)
        {
            // Kiểm tra lịch sử bệnh án tồn tại
            var existingRecord = _medicalRecordHistoryRepository.GetMedicalRecordHistory(medicalRecordDTO.MedicalRecordHistoryId);
            if (existingRecord == null)
                throw new Exception("Lịch sử bệnh án không tồn tại!");

            // 1. Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.MedicalRecordHistoryCode))
                throw new Exception("Mã bệnh án không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.PatientCategory))
                throw new Exception("Đối tượng không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.DiagnoseConclusion))
                throw new Exception("Kết luận chẩn đoán không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.TreatmentMethod))
                throw new Exception("Phương pháp điều trị không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.DiseaseProgress))
                throw new Exception("Diễn biến bệnh không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.MedicalOrder))
                throw new Exception("Y lệnh không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.Symptom))
                throw new Exception("Triệu chứng không được để trống!");
            if (!medicalRecordDTO.Pulse.HasValue)
                throw new Exception("Mạch không được để trống!");
            if (!medicalRecordDTO.RespiratoryRate.HasValue)
                throw new Exception("Nhịp thở không được để trống!");
            if (!medicalRecordDTO.Temperature.HasValue)
                throw new Exception("Nhiệt độ không được để trống!");
            if (!medicalRecordDTO.Height.HasValue)
                throw new Exception("Chiều cao không được để trống!");
            if (!medicalRecordDTO.Weight.HasValue)
                throw new Exception("Cân nặng không được để trống!");
            if (string.IsNullOrWhiteSpace(medicalRecordDTO.BloodPressure))
                throw new Exception("Huyết áp không được để trống!");

            // 2. Validate ngưỡng sinh lý
            if (medicalRecordDTO.Pulse.HasValue && (medicalRecordDTO.Pulse < 30 || medicalRecordDTO.Pulse > 200))
                throw new Exception("Mạch phải nằm trong khoảng 30 bpm đến 200 bpm!");
            if (medicalRecordDTO.RespiratoryRate.HasValue && (medicalRecordDTO.RespiratoryRate < 10 || medicalRecordDTO.RespiratoryRate > 60))
                throw new Exception("Nhịp thở phải nằm trong khoảng 10 lần/phút đến 60 lần/phút!");
            if (medicalRecordDTO.Temperature.HasValue && (medicalRecordDTO.Temperature < 33 || medicalRecordDTO.Temperature > 45))
                throw new Exception("Nhiệt độ phải nằm trong khoảng 33°C đến 45°C!");
            if (medicalRecordDTO.Height.HasValue && (medicalRecordDTO.Height < 30 || medicalRecordDTO.Height > 250))
                throw new Exception("Chiều cao phải nằm trong khoảng 30 cm đến 250 cm!");
            if (medicalRecordDTO.Weight.HasValue && (medicalRecordDTO.Weight < 1 || medicalRecordDTO.Weight > 400))
                throw new Exception("Cân nặng phải nằm trong khoảng 1 kg đến 300 kg!");

            // 3. Validate huyết áp
            if (!string.IsNullOrWhiteSpace(medicalRecordDTO.BloodPressure) && !Regex.IsMatch(medicalRecordDTO.BloodPressure, @"^\d{1,3}/\d{1,3}$"))
                throw new Exception("Huyết áp phải có định dạng 'số/số' (ví dụ: 120/80)!");


            // Gán các trường có thể được cập nhật (nếu muốn, có thể check null để chỉ update khi có giá trị)
            existingRecord.Address = medicalRecordDTO.Address;
            existingRecord.MedicalRecordId = medicalRecordDTO.PatientId;
            existingRecord.DiagnoseConclusion = medicalRecordDTO.DiagnoseConclusion;
            existingRecord.TreatmentMethod = medicalRecordDTO.TreatmentMethod;
            existingRecord.Symptom = medicalRecordDTO.Symptom;
            existingRecord.Date = medicalRecordDTO.RecordDate;
            existingRecord.Pulse = medicalRecordDTO.Pulse;
            existingRecord.BloodPressure = medicalRecordDTO.BloodPressure;
            existingRecord.RespiratoryRate = medicalRecordDTO.RespiratoryRate;
            existingRecord.Temperature = medicalRecordDTO.Temperature;
            existingRecord.Height = medicalRecordDTO.Height;
            existingRecord.Weight = medicalRecordDTO.Weight;
            existingRecord.Note = medicalRecordDTO.Note;
            existingRecord.MedicalRecordHistoryCode = medicalRecordDTO.MedicalRecordHistoryCode;
            existingRecord.PatientCategory = medicalRecordDTO.PatientCategory;
            existingRecord.DiseaseProgress = medicalRecordDTO.DiseaseProgress;
            existingRecord.DiseaseStage = medicalRecordDTO.DiseaseStage;
            existingRecord.Icd = medicalRecordDTO.ICD;
            existingRecord.MedicalOrder = medicalRecordDTO.MedicalOrder;
            existingRecord.TreatmentBed = medicalRecordDTO.TreatmentBed;

            return _medicalRecordHistoryRepository.UpdateMedicalRecordHistory(existingRecord);
        }


        /*        public bool DeleteMedicalRecordHistory(int medicalRecordId)
                {
                    return _medicalRecordHistoryRepository.DeleteMedicalRecordHistory(medicalRecordId);
                }*/

        public int GetTodayMedicalRecordHistoryCount()
        {
            return _medicalRecordHistoryRepository.CountTodayMedicalRecordHistories();
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
