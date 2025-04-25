using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace CHSMS.API.Test.ExternalPrescriptionTest
{
    public class GetExternalPrescriptionsByMedicalRecordHistoryIdAsyncTests : IDisposable
    {
        private readonly Mock<IExternalPrescriptionRepository> _repositoryMock;
        private readonly SEP_TestContext _dbContext;
        private readonly ExternalPrescriptionService _service;

        public GetExternalPrescriptionsByMedicalRecordHistoryIdAsyncTests()
        {
            _repositoryMock = new Mock<IExternalPrescriptionRepository>();

            var options = new DbContextOptionsBuilder<SEP_TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new SEP_TestContext(options);
            _service = new ExternalPrescriptionService(_repositoryMock.Object, _dbContext);
        }

        // Helper Methods
        private List<ExternalPrescription> CreateDefaultPrescriptions(int medicalRecordHistoryId)
        {
            return new List<ExternalPrescription>
            {
                new ExternalPrescription
                {
                    ExternalPrescriptionId = 1,
                    MedicalRecordHistoryId = medicalRecordHistoryId,
                    IssueDate = DateTime.Now.Date,
                    Status = true,
                    Note = "Prescription 1",
                    IsBhyt = false,
                    MedicalRecordHistory = new MedicalRecordHistory
                    {
                        MedicalRecord = new MedicalRecord { PatientName = "John Doe" }
                    }
                },
                new ExternalPrescription
                {
                    ExternalPrescriptionId = 2,
                    MedicalRecordHistoryId = medicalRecordHistoryId,
                    IssueDate = DateTime.Now.Date.AddDays(-1),
                    Status = false,
                    Note = "Prescription 2",
                    IsBhyt = true,
                    MedicalRecordHistory = new MedicalRecordHistory
                    {
                        MedicalRecord = new MedicalRecord { PatientName = "Jane Smith" }
                    }
                }
            };
        }

        [Fact]
        public async Task GetExternalPrescriptionsByMedicalRecordHistoryIdAsync_ValidId_ReturnsPrescriptionDTOs()
        {
            // Arrange
            int medicalRecordHistoryId = 1;
            var prescriptions = CreateDefaultPrescriptions(medicalRecordHistoryId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId))
                .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            var firstDto = result[0];
            Assert.Equal(1, firstDto.ExternalPrescriptionId);
            Assert.Equal(prescriptions[0].IssueDate, firstDto.IssueDate);
            Assert.True(firstDto.Status);
            Assert.Equal("Prescription 1", firstDto.Note);
            Assert.False(firstDto.IsBhyt);
            Assert.Equal("John Doe", firstDto.PatientName);

            var secondDto = result[1];
            Assert.Equal(2, secondDto.ExternalPrescriptionId);
            Assert.Equal(prescriptions[1].IssueDate, secondDto.IssueDate);
            Assert.False(secondDto.Status);
            Assert.Equal("Prescription 2", secondDto.Note);
            Assert.True(secondDto.IsBhyt);
            Assert.Equal("Jane Smith", secondDto.PatientName);

            _repositoryMock.Verify(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId), Times.Once());
        }

        [Fact]
        public async Task GetExternalPrescriptionsByMedicalRecordHistoryIdAsync_NoPrescriptions_ThrowsException()
        {
            // Arrange
            int medicalRecordHistoryId = 1;
            _repositoryMock.Setup(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId))
                .ReturnsAsync(new List<ExternalPrescription>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId));
            Assert.Equal("Không tìm thấy đơn thuốc", exception.Message);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId), Times.Once());
        }

        [Fact]
        public async Task GetExternalPrescriptionsByMedicalRecordHistoryIdAsync_NullPrescriptions_ThrowsException()
        {
            // Arrange
            int medicalRecordHistoryId = 1;
            _repositoryMock.Setup(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId))
                .ReturnsAsync((List<ExternalPrescription>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId));
            Assert.Equal("Không tìm thấy đơn thuốc", exception.Message);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId), Times.Once());
        }

        [Fact]
        public async Task GetExternalPrescriptionsByMedicalRecordHistoryIdAsync_NullProperties_ReturnsDefaultValues()
        {
            // Arrange
            int medicalRecordHistoryId = 1;
            var prescriptions = new List<ExternalPrescription>
            {
                new ExternalPrescription
                {
                    ExternalPrescriptionId = 1,
                    MedicalRecordHistoryId = medicalRecordHistoryId,
                    IssueDate = null,
                    Status = null,
                    Note = null,
                    IsBhyt = null,
                    MedicalRecordHistory = null
                }
            };

            _repositoryMock.Setup(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId))
                .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var dto = result[0];
            Assert.Equal(1, dto.ExternalPrescriptionId);
            Assert.Equal(DateTime.MinValue, dto.IssueDate);
            Assert.False(dto.Status);
            Assert.Equal(string.Empty, dto.Note);
            Assert.False(dto.IsBhyt);
            Assert.Null(dto.PatientName);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId), Times.Once());
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}