using CHSMS.API.Models;

namespace CHSMS.API.Tests.Services
{
    public class GetAllMedicalRecordHistoriesTests : MedicalRecordHistoryServiceTestBase
    {
        [Fact]
        public void GetAllMedicalRecordHistories_WithData_ReturnsListOfDTOs()
        {
            // Arrange
            var testHistory = CreateTestMedicalRecordHistory();
            var testUser = CreateTestUser();
            var testMedicalRecord = CreateTestMedicalRecord();

            testHistory.User = testUser;
            testHistory.MedicalRecord = testMedicalRecord;

            var histories = new List<MedicalRecordHistory> { testHistory };
            _mockHistoryRepo.Setup(x => x.GetAllMedicalRecordHistories()).Returns(histories);

            // Act
            var result = _service.GetAllMedicalRecordHistories();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(testHistory.MedicalRecordHistoryId, result[0].MedicalRecordHistoryId);
            Assert.Equal(testUser.UserName, result[0].DoctorName);
            Assert.Equal(testMedicalRecord.PatientName, result[0].PatientName);
        }

        [Fact]
        public void GetAllMedicalRecordHistories_WithoutData_ReturnsNull()
        {
            // Arrange
            _mockHistoryRepo.Setup(x => x.GetAllMedicalRecordHistories())
                .Returns(new List<MedicalRecordHistory>());

            // Act
            var result = _service.GetAllMedicalRecordHistories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}