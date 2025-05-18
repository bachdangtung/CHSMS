using CHSMS.API.Tests.Services;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetTodayMedicalRecordHistoryCountTests : MedicalRecordHistoryServiceTestBase
    {
        [Fact]
        public void GetTodayMedicalRecordHistoryCount_5Rcecord_ReturnsCorrectCount()
        {
            // Arrange
            _mockHistoryRepo.Setup(x => x.CountTodayMedicalRecordHistories()).Returns(5);

            // Act
            var result = _service.GetTodayMedicalRecordHistoryCount();

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void GetTodayMedicalRecordHistoryCount_NoRcecord_ReturnsCorrectCount()
        {
            // Arrange
            _mockHistoryRepo.Setup(x => x.CountTodayMedicalRecordHistories()).Returns(0);

            // Act
            var result = _service.GetTodayMedicalRecordHistoryCount();

            // Assert
            Assert.Equal(5, result);
        }
    }
}
