using CHSMS.API.DTOs.ExternalPrescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace CHSMS.API.Test.ExternalPrescriptionTest
{
    public class CreateExternalPrescriptionAsyncTests : IDisposable
    {
        private readonly Mock<IExternalPrescriptionRepository> _repositoryMock;
        private readonly SEP_TestContext _dbContext;
        private readonly ExternalPrescriptionService _service;

        public CreateExternalPrescriptionAsyncTests()
        {
            _repositoryMock = new Mock<IExternalPrescriptionRepository>();

            var options = new DbContextOptionsBuilder<SEP_TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new SEP_TestContext(options);
            _service = new ExternalPrescriptionService(_repositoryMock.Object, _dbContext);
        }

        [Fact]
        public async Task CreateExternalPrescriptionAsync_ValidInput_ReturnsPrescriptionId()
        {
            // Arrange
            int userId = 1;
            int medicalRecordHistoryId = 1;
            var dto = CreateDefaultPrescriptionDTO();
            var validMedicines = CreateDefaultValidMedicines();
            var createdPrescription = CreateDefaultCreatedPrescription(1, medicalRecordHistoryId, userId, dto);

            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(validMedicines);
            _repositoryMock.Setup(r => r.CreateExternalPrescriptionAsync(It.IsAny<ExternalPrescription>()))
                .ReturnsAsync(createdPrescription);
            _repositoryMock.Setup(r => r.CreateExternalMedicinePrescriptionAsync(It.IsAny<MedicinePrescription>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateExternalPrescriptionAsync(userId, medicalRecordHistoryId, dto);

            // Assert
            Assert.Equal(1, result);
            _repositoryMock.Verify(r => r.CreateExternalPrescriptionAsync(It.IsAny<ExternalPrescription>()), Times.Once());
            _repositoryMock.Verify(r => r.CreateExternalMedicinePrescriptionAsync(It.IsAny<MedicinePrescription>()), Times.Once());
        }

        [Fact]
        public async Task CreateExternalPrescriptionAsync_FutureIssueDate_ThrowsException()
        {
            // Arrange
            int userId = 1;
            int medicalRecordHistoryId = 1;
            var dto = CreateDefaultPrescriptionDTO(issueDate: DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreateExternalPrescriptionAsync(userId, medicalRecordHistoryId, dto));
            Assert.Contains("Ngày phát hành không được là ngày trong tương lai!", exception.Message);
        }

        [Fact]
        public async Task CreateExternalPrescriptionAsync_TooManyMedicines_ThrowsException()
        {
            // Arrange
            int userId = 1;
            int medicalRecordHistoryId = 1;
            var medicines = Enumerable.Range(1, 11)
                .Select(i => new MedicinePrescriptionDTO { MedicineId = i, Amount = 10, Note = "Take daily" })
                .ToList();
            var dto = CreateDefaultPrescriptionDTO(medicines: medicines);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreateExternalPrescriptionAsync(userId, medicalRecordHistoryId, dto));
            Assert.Contains("Một đơn thuốc không được chứa quá 10 loại thuốc!", exception.Message);
        }

        [Fact]
        public async Task CreateExternalPrescriptionAsync_DuplicateMedicineIds_ThrowsException()
        {
            // Arrange
            int userId = 1;
            int medicalRecordHistoryId = 1;
            var dto = CreateDefaultPrescriptionDTO();
            dto.MedicinesToAdd.Add(dto.MedicinesToAdd[0]); // duplicate

            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(CreateDefaultValidMedicines());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreateExternalPrescriptionAsync(userId, medicalRecordHistoryId, dto));
            Assert.Contains("bị trùng", ex.Message);
        }

        [Fact]
        public async Task CreateExternalPrescriptionAsync_InvalidMedicineId_ThrowsException()
        {
            // Arrange
            int userId = 1;
            int medicalRecordHistoryId = 1;
            var dto = CreateDefaultPrescriptionDTO();

            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(new List<Medicine>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreateExternalPrescriptionAsync(userId, medicalRecordHistoryId, dto));
            Assert.Contains("Không tìm thấy thuốc với ID: 1 hoặc thuốc không hoạt động!", exception.Message);
        }

        #region Helper
        private CreateExternalPrescriptionDTO CreateDefaultPrescriptionDTO(
            DateTime? issueDate = null,
            string note = "Test prescription",
            bool isBhyt = false,
            List<MedicinePrescriptionDTO> medicines = null)
        {
            return new CreateExternalPrescriptionDTO
            {
                IssueDate = issueDate ?? DateTime.Now.AddDays(-1),
                Note = note,
                IsBhyt = isBhyt,
                MedicinesToAdd = medicines ?? new List<MedicinePrescriptionDTO>
                {
                    new MedicinePrescriptionDTO { MedicineId = 1, Amount = 10, Note = "Take daily" }
                }
            };
        }

        private List<Medicine> CreateDefaultValidMedicines()
        {
            return new List<Medicine>
            {
                new Medicine { MedicineId = 1, Status = true }
            };
        }

        private ExternalPrescription CreateDefaultCreatedPrescription(
            int externalPrescriptionId,
            int medicalRecordHistoryId,
            int userId,
            CreateExternalPrescriptionDTO dto)
        {
            return new ExternalPrescription
            {
                ExternalPrescriptionId = externalPrescriptionId,
                MedicalRecordHistoryId = medicalRecordHistoryId,
                UserId = userId,
                IssueDate = dto.IssueDate,
                Status = true,
                Note = dto.Note,
                IsBhyt = false
            };
        }
        #endregion


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}