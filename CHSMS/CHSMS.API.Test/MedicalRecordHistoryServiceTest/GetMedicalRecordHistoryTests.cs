using CHSMS.API.Models;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetMedicalRecordHistoryTests : MedicalRecordHistoryServiceTestBase
    {
        [Fact]
        public void GetMedicalRecordHistory_ValidId_ReturnsDTO()
        {
            // Arrange
            var testHistory = CreateTestMedicalRecordHistory();
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(testHistory);

            // Act
            var result = _service.GetMedicalRecordHistory(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testHistory.MedicalRecordHistoryId, result.MedicalRecordHistoryId);
        }

        [Fact]
        public void GetMedicalRecordHistory_InvalidId_ReturnsNull()
        {
            // Arrange
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(-1)).Returns((MedicalRecordHistory)null);

            // Act
            var result = _service.GetMedicalRecordHistory(999);

            // Assert
            Assert.Null(result);
        }
    }
}
