using CHSMS.API.Models;
using CHSMS.API.Tests.Services;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetMedicalRecordHistoryByPatientIdTests : MedicalRecordHistoryServiceTestBase
    {
        public class MedicalRecordHistoryService_GetByPatientIdTests : MedicalRecordHistoryServiceTestBase
        {
            private readonly MedicalRecordHistory _testHistory;
            private readonly DateTime _startDate = new DateTime(2024, 12, 12);
            private readonly DateTime _endDate = new DateTime(2025, 12, 12);
            private readonly DateTime _recordDate = new DateTime(2025, 1, 1);

            public MedicalRecordHistoryService_GetByPatientIdTests()
            {
                _testHistory = new MedicalRecordHistory
                {
                    MedicalRecordHistoryId = 1,
                    MedicalRecordId = 1,
                    Date = _recordDate,
                    User = new User { Fullname = "Huyen" }
                };
            }

            [Fact] // Test case 1: All filters null
            public void GetMedicalRecordHistoryByPatientId_AllFiltersNull_ReturnsAllRecords()
            {
                // Arrange
                var histories = new List<MedicalRecordHistory> { _testHistory };
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(1, null, null, null))
                    .Returns(histories);

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(1, null, null, null);

                // Assert
                Assert.Single(result);
                Assert.Equal(1, result[0].MedicalRecordHistoryId);
            }

            [Fact] // Test case 2: Valid PatientId, startDate only
            public void GetMedicalRecordHistoryByPatientId_WithStartDate_ReturnsFilteredRecords()
            {
                // Arrange
                var histories = new List<MedicalRecordHistory> { _testHistory };
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(1, _startDate, null, null))
                    .Returns(histories);

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(1, _startDate, null, null);

                // Assert
                Assert.Single(result);
                Assert.Equal(_recordDate, result[0].RecordDate);
            }

            [Fact] // Test case 3: Valid PatientId, endDate only
            public void GetMedicalRecordHistoryByPatientId_WithEndDate_ReturnsFilteredRecords()
            {
                // Arrange
                var histories = new List<MedicalRecordHistory> { _testHistory };
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(1, null, _endDate, null))
                    .Returns(histories);

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(1, null, _endDate, null);

                // Assert
                Assert.Single(result);
                Assert.Equal(_recordDate, result[0].RecordDate);
            }

            [Fact] // Test case 4: Valid PatientId, doctorName only
            public void GetMedicalRecordHistoryByPatientId_WithDoctorName_ReturnsFilteredRecords()
            {
                // Arrange
                var histories = new List<MedicalRecordHistory> { _testHistory };
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(1, null, null, "Huyen"))
                    .Returns(histories);

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(1, null, null, "Huyen");

                // Assert
                Assert.Single(result);
                Assert.Equal("Huyen", result[0].Fullname);
            }

            [Fact] // Test case 5: Valid PatientId, startDate and endDate
            public void GetMedicalRecordHistoryByPatientId_WithDateRange_ReturnsFilteredRecords()
            {
                // Arrange
                var histories = new List<MedicalRecordHistory> { _testHistory };
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(1, _startDate, _endDate, null))
                    .Returns(histories);

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(1, _startDate, _endDate, null);

                // Assert
                Assert.Single(result);
                Assert.True(result[0].RecordDate >= _startDate && result[0].RecordDate <= _endDate);
            }

            [Fact] // Test case 6: Valid PatientId, all filters
            public void GetMedicalRecordHistoryByPatientId_WithAllFilters_ReturnsFilteredRecords()
            {
                // Arrange
                var histories = new List<MedicalRecordHistory> { _testHistory };
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(1, _startDate, _endDate, "Huyen"))
                    .Returns(histories);

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(1, _startDate, _endDate, "Huyen");

                // Assert
                Assert.Single(result);
                Assert.Equal("Huyen", result[0].Fullname);
                Assert.True(result[0].RecordDate >= _startDate && result[0].RecordDate <= _endDate);
            }

            [Fact] // Edge case: Invalid PatientId
            public void GetMedicalRecordHistoryByPatientId_InvalidPatientId_ReturnsEmptyList()
            {
                // Arrange
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(-1, null, null, null))
                    .Returns(new List<MedicalRecordHistory>());

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(-1, null, null, null);

                // Assert
                Assert.Empty(result);
            }

            [Fact] // Edge case: No matching records
            public void GetMedicalRecordHistoryByPatientId_NoMatchingRecords_ReturnsEmptyList()
            {
                // Arrange
                _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistoryByPatientId(1, _startDate, _endDate, "NonExisting"))
                    .Returns(new List<MedicalRecordHistory>());

                // Act
                var result = _service.GetMedicalRecordHistoryByPatientId(1, _startDate, _endDate, "NonExisting");

                // Assert
                Assert.Empty(result);
            }
        }
    }
}
