using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetPrescriptionsByMedicalRecordHistoryIdAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetPrescriptionsByMedicalRecordHistoryIdAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetPrescriptionsByMedicalRecordHistoryIdAsync_NoPrescriptions_ThrowsException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetPrescriptionsByMedicalRecordHistoryIdAsync(1))
                           .ReturnsAsync(new List<Prescription>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetPrescriptionsByMedicalRecordHistoryIdAsync(1));
            Assert.Contains("Không tìm thấy đơn thuốc", exception.Message);
        }

        [Fact]
        public async Task GetPrescriptionsByMedicalRecordHistoryIdAsync_ValidId_ReturnsMappedList()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                new Prescription
                {
                    PrescriptionId = 1,
                    IssueDate = DateTime.Now,
                    Status = true,
                    MedicalRecordHistory = new MedicalRecordHistory
                    {
                        MedicalRecord = new MedicalRecord { PatientName = "John Doe" }
                    }
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionsByMedicalRecordHistoryIdAsync(1))
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetPrescriptionsByMedicalRecordHistoryIdAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].PrescriptionId);
            Assert.Equal("John Doe", result[0].PatientName);
        }
        [Fact]
        public async Task GetPrescriptionsByMedicalRecordHistoryIdAsync_MedicalRecordHistoryNull_ReturnsWithNullPatientName()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                new Prescription
                {
                    PrescriptionId = 2,
                    IssueDate = DateTime.Now,
                    Status = true,
                    MedicalRecordHistory = null
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionsByMedicalRecordHistoryIdAsync(1))
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetPrescriptionsByMedicalRecordHistoryIdAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result[0].PrescriptionId);
            Assert.Null(result[0].PatientName);
        }
        [Fact]
        public async Task GetPrescriptionsByMedicalRecordHistoryIdAsync_MedicalRecordNull_ReturnsWithNullPatientName()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                new Prescription
                {
                    PrescriptionId = 3,
                    IssueDate = DateTime.Now,
                    Status = true,
                    MedicalRecordHistory = new MedicalRecordHistory
                    {
                        MedicalRecord = null
                    }
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionsByMedicalRecordHistoryIdAsync(1))
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetPrescriptionsByMedicalRecordHistoryIdAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(3, result[0].PrescriptionId);
            Assert.Null(result[0].PatientName);
        }
        
    }
}