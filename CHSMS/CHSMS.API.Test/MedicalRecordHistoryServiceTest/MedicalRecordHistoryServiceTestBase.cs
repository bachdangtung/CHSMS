using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Tests.Services
{
    public class MedicalRecordHistoryServiceTestBase
    {
        protected readonly Mock<IMedicalRecordHistoryRepository> _mockHistoryRepo;
        protected readonly Mock<IUserRepository> _mockUserRepo;
        protected readonly Mock<IMedicalRecordRepository> _mockMedicalRecordRepo;
        protected readonly MedicalRecordHistoryService _service;

        public MedicalRecordHistoryServiceTestBase()
        {
            _mockHistoryRepo = new Mock<IMedicalRecordHistoryRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMedicalRecordRepo = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordHistoryService(
                _mockHistoryRepo.Object,
                _mockUserRepo.Object,
                _mockMedicalRecordRepo.Object);
        }

        protected MedicalRecordHistoryDTO CreateValidMedicalRecordHistoryDTO()
        {
            return new MedicalRecordHistoryDTO
            {
                PatientId = 1,
                RecordDate = DateTime.Now,
                Pulse = 72,
                BloodPressure = "120/80",
                RespiratoryRate = 16,
                Temperature = 36.5,
                Height = 170,
                Weight = 70,
                DiagnoseConclusion = "Test Diagnosis",
                TreatmentMethod = "Test Treatment",
                Symptom = "Test Symptom",
                MedicalRecordHistoryCode = "MRH001",
                PatientCategory = "Outpatient",
                DiseaseProgress = "Stable",
                MedicalOrder = "Test Order"
            };
        }

        protected MedicalRecordHistory CreateTestMedicalRecordHistory()
        {
            return new MedicalRecordHistory
            {
                MedicalRecordHistoryId = 1,
                MedicalRecordId = 1,
                UserId = 1,
                DiagnoseConclusion = "Test Diagnosis",
                TreatmentMethod = "Test Treatment",
                Date = DateTime.Now,
                Pulse = 72,
                BloodPressure = "120/80",
                RespiratoryRate = 16,
                Temperature = 36.5,
                Height = 170,
                Weight = 70,
                Symptom = "Test Symptom",
                MedicalRecordHistoryCode = "MRH001",
                PatientCategory = "Outpatient",
                DiseaseProgress = "Stable",
                MedicalOrder = "Test Order"
            };
        }

        protected MedicalRecord CreateTestMedicalRecord()
        {
            return new MedicalRecord
            {
                MedicalRecordId = 1,
                PatientName = "Test Patient",
                Gender = "Male",
                Dob = new DateTime(1980, 1, 1),
                EthnicGroup = "Kinh",
                EducationLevel = "University",
                Address = "Test Address",
                Job = "Engineer"
            };
        }

        protected User CreateTestUser()
        {
            return new User
            {
                UserId = 1,
                UserName = "doctor1",
                Fullname = "Dr. Test"
            };
        }

        protected MedicalRecordDTO CreateValidMedicalRecordDTO()
        {
            return new MedicalRecordDTO
            {
                PatientName = "Test Patient",
                Gender = "Male",
                Dob = new DateTime(1980, 1, 1),
                EthnicGroup = "Kinh",
                EducationLevel = "University",
                Address = "Test Address",
                Job = "Engineer"
            };
        }
    }
}