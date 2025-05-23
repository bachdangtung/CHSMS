using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class MedicalRecordHistoryService_GetTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _mockHistoryRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IMedicalRecordRepository> _mockMedicalRecordRepo;
        private readonly MedicalRecordHistoryService _service;

        public MedicalRecordHistoryService_GetTests()
        {
            _mockHistoryRepo = new Mock<IMedicalRecordHistoryRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMedicalRecordRepo = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordHistoryService(
                _mockHistoryRepo.Object,
                _mockUserRepo.Object,
                _mockMedicalRecordRepo.Object);
        }

        [Fact]
        public void GetAllMedicalRecordHistories_ShouldReturnAllRecords()
        {
            // Arrange
            var testRecords = new List<MedicalRecordHistory>
            {
                new() { MedicalRecordHistoryId = 1, UserId = 1, MedicalRecordId = 1 },
                new() { MedicalRecordHistoryId = 2, UserId = 2, MedicalRecordId = 2 }
            };

            _mockHistoryRepo.Setup(repo => repo.GetAllMedicalRecordHistories())
                .Returns(testRecords);

            // Act
            var result = _service.GetAllMedicalRecordHistories();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].MedicalRecordHistoryId);
            Assert.Equal(2, result[1].MedicalRecordHistoryId);
        }

        [Fact]
        public void GetMedicalRecordHistory_WithValidId_ShouldReturnRecord()
        {
            // Arrange
            var testRecord = new MedicalRecordHistory
            {
                MedicalRecordHistoryId = 1,
                UserId = 1,
                MedicalRecordId = 1,
                DiagnoseConclusion = "Test Diagnosis"
            };

            _mockHistoryRepo.Setup(repo => repo.GetMedicalRecordHistory(1))
                .Returns(testRecord);

            // Act
            var result = _service.GetMedicalRecordHistory(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MedicalRecordHistoryId);
            Assert.Equal("Test Diagnosis", result.DiagnoseConclusion);
        }

        [Fact]
        public void GetMedicalRecordHistory_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockHistoryRepo.Setup(repo => repo.GetMedicalRecordHistory(It.IsAny<int>()))
                .Returns((MedicalRecordHistory)null);

            // Act
            var result = _service.GetMedicalRecordHistory(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetMedicalRecordHistoryByPatientId_ShouldReturnFilteredRecords()
        {
            // Arrange
            var testRecords = new List<MedicalRecordHistory>
            {
                new() { MedicalRecordHistoryId = 1, MedicalRecordId = 1 },
                new() { MedicalRecordHistoryId = 2, MedicalRecordId = 1 },
                new() { MedicalRecordHistoryId = 3, MedicalRecordId = 2 }
            };

            _mockHistoryRepo.Setup(repo => repo.GetMedicalRecordHistoryByPatientId(
                It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>()))
                .Returns(testRecords.Where(r => r.MedicalRecordId == 1).ToList());

            // Act
            var result = _service.GetMedicalRecordHistoryByPatientId(1, null, null, null);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(1, r.PatientId));
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_ShouldReturnFilteredRecords()
        {
            // Arrange
            var testRecords = new List<MedicalRecordHistory>
            {
                new() {
                    MedicalRecordHistoryId = 1,
                    MedicalRecord = new MedicalRecord { PatientName = "John Doe" },
                    User = new User { UserName = "dr_smith" }
                },
                new() {
                    MedicalRecordHistoryId = 2,
                    MedicalRecord = new MedicalRecord { PatientName = "Jane Doe" },
                    User = new User { UserName = "dr_jones" }
                }
            };

            _mockHistoryRepo.Setup(repo => repo.GetMedicalRecordHistoriesByFilter(
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(testRecords.Where(r => r.MedicalRecord.PatientName.Contains("Doe")).ToList());

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter(null, "Doe");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.PatientName == "John Doe");
            Assert.Contains(result, r => r.PatientName == "Jane Doe");
        }

        [Fact]
        public void GetTodayMedicalRecordHistoryCount_ShouldReturnCorrectCount()
        {
            // Arrange
            _mockHistoryRepo.Setup(repo => repo.CountTodayMedicalRecordHistories())
                .Returns(5);

            // Act
            var result = _service.GetTodayMedicalRecordHistoryCount();

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var testUsers = new List<User>
            {
                new() { UserId = 1, UserName = "user1", Gender = "Male" },
                new() { UserId = 2, UserName = "user2", Gender = "Female" }
            };

            _mockHistoryRepo.Setup(repo => repo.GetAllUsers())
                .Returns(testUsers);

            // Act
            var result = _service.GetAllUsers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("user1", result[0].UserName);
            Assert.Equal("user2", result[1].UserName);
        }
    }
}
