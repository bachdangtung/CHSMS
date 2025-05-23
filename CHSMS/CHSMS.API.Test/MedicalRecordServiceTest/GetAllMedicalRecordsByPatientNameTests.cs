using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class GetAllMedicalRecordsByPatientNameTests
    {
        private readonly MedicalRecordService _service;
        private readonly Mock<IMedicalRecordRepository> _mockRepo;

        public GetAllMedicalRecordsByPatientNameTests()
        {
            _mockRepo = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepo.Object);

            // Setup test data matching the precondition
            var testRecords = new List<MedicalRecord>
        {
            new MedicalRecord
            {
                MedicalRecordId = 1,
                PatientName = "John",
                HealthInsurance = "DN2790123456789",
                Gender = "Male",
                Dob = new DateTime(1980, 1, 1)
            }
        };

            // Setup mock repository to return records based on filters
            _mockRepo.Setup(x => x.GetMedicalRecordsByPatientName(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns((string name, string insurance) =>
                        testRecords.FindAll(r =>
                            (name == null || r.PatientName == name) &&
                            (insurance == null || r.HealthInsurance == insurance)));
        }

        // Test case 1: PatientName = John, HealthInsurance = DN2790123456789 (○ ○ ○ ○)
        [Fact]
        public void Case1_FilterByNameAndInsurance_ReturnsRecord()
        {
            var result = _service.GetAllMedicalRecordsByPatientName("John", "DN2790123456789");
            Assert.Single(result);
            Assert.Equal("John", result[0].PatientName);
            Assert.Equal("DN2790123456789", result[0].HealthInsurance);
        }

        // Test case 2: PatientName = NonExistent, HealthInsurance = null (✓ ✓ ○ ✓)
        [Fact]
        public void Case2_FilterByNonExistentName_ReturnsEmpty()
        {
            var result = _service.GetAllMedicalRecordsByPatientName("NonExistent", null);
            Assert.Empty(result);
        }

        // Test case 3: PatientName = null, HealthInsurance = DN2790123456789 (✓ ✓ ✓ ○)
        [Fact]
        public void Case3_FilterByInsuranceOnly_ReturnsRecord()
        {
            var result = _service.GetAllMedicalRecordsByPatientName(null, "DN2790123456789");
            Assert.Single(result);
            Assert.Equal("DN2790123456789", result[0].HealthInsurance);
        }

        // Test case 4: PatientName = John, HealthInsurance = null (○ ○ ○ ✓)
        [Fact]
        public void Case4_FilterByNameOnly_ReturnsRecord()
        {
            var result = _service.GetAllMedicalRecordsByPatientName("John", null);
            Assert.Single(result);
            Assert.Equal("John", result[0].PatientName);
        }

        // Test case 5: PatientName = null, HealthInsurance = DN3790987654321 (○ ✓ ✓ ✓)
        [Fact]
        public void Case5_FilterByNonMatchingInsurance_ReturnsEmpty()
        {
            var result = _service.GetAllMedicalRecordsByPatientName(null, "DN3790987654321");
            Assert.Empty(result);
        }

        // Test case 6: PatientName = null, HealthInsurance = null (✓ ✓ ○ ✓)
        [Fact]
        public void Case6_NoFilters_ReturnsRecord()
        {
            var result = _service.GetAllMedicalRecordsByPatientName(null, null);
            Assert.Single(result);
        }
    }
}
