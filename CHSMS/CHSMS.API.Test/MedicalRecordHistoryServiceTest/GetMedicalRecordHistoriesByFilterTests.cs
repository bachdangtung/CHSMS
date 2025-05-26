using CHSMS.API.Models;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class MedicalRecordHistoryService_FilterTests : MedicalRecordHistoryServiceTestBase
    {
        [Fact]
        public void GetMedicalRecordHistoriesByFilter_BothDoctorNameAndPatientName_ReturnsFilteredResults()
        {
            // Arrange
            var testHistory = CreateTestMedicalRecordHistory();
            testHistory.User = new User { Fullname = "Huven", UserName = "doctor1" };
            testHistory.MedicalRecord = new MedicalRecord { PatientName = "Van" };

            var expectedResult = new List<MedicalRecordHistory> { testHistory };

            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoriesByFilter("Huven", "Van"))
                .Returns(expectedResult);

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter("Huven", "Van");

            // Assert
            Assert.Single(result);
            Assert.Equal("Huven", result[0].Fullname);
            Assert.Equal("Van", result[0].PatientName);
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_OnlyDoctorName_ReturnsFilteredResults()
        {
            // Arrange
            var testHistory = CreateTestMedicalRecordHistory();
            testHistory.User = new User { Fullname = "Huven", UserName = "doctor1" };
            testHistory.MedicalRecord = new MedicalRecord { PatientName = "Van" };

            var expectedResult = new List<MedicalRecordHistory> { testHistory };

            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoriesByFilter("Huven", null))
                .Returns(expectedResult);

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter("Huven", null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Huven", result[0].Fullname);
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_OnlyPatientName_ReturnsFilteredResults()
        {
            // Arrange
            var testHistory = CreateTestMedicalRecordHistory();
            testHistory.User = new User { Fullname = "Huven", UserName = "doctor1" };
            testHistory.MedicalRecord = new MedicalRecord { PatientName = "Van" };

            var expectedResult = new List<MedicalRecordHistory> { testHistory };

            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoriesByFilter(null, "Van"))
                .Returns(expectedResult);

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter(null, "Van");

            // Assert
            Assert.Single(result);
            Assert.Equal("Van", result[0].PatientName);
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_NoFilters_ReturnsAllResults()
        {
            // Arrange
            var testHistory1 = CreateTestMedicalRecordHistory();
            testHistory1.User = new User { Fullname = "Huven", UserName = "doctor1" };
            testHistory1.MedicalRecord = new MedicalRecord { PatientName = "Van" };

            var testHistory2 = CreateTestMedicalRecordHistory();
            testHistory2.MedicalRecordHistoryId = 2;
            testHistory2.User = new User { Fullname = "John", UserName = "doctor2" };
            testHistory2.MedicalRecord = new MedicalRecord { PatientName = "Doe" };

            var expectedResult = new List<MedicalRecordHistory> { testHistory1, testHistory2 };

            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoriesByFilter(null, null))
                .Returns(expectedResult);

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter(null, null);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_NoMatches_ReturnsEmptyList()
        {
            // Arrange
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoriesByFilter("NonExisting", "Patient"))
                .Returns(new List<MedicalRecordHistory>());

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter("NonExisting", "Patient");

            // Assert
            Assert.Empty(result);
        }
    }
}