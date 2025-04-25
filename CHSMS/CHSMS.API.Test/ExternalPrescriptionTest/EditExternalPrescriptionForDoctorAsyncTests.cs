using CHSMS.API.DTOs.ExternalPrescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace CHSMS.API.Test.ExternalPrescriptionTest
{
    public class EditExternalPrescriptionForDoctorAsyncTests : IDisposable
    {
        private readonly Mock<IExternalPrescriptionRepository> _repositoryMock;
        private readonly SEP_TestContext _dbContext;
        private readonly ExternalPrescriptionService _service;

        public EditExternalPrescriptionForDoctorAsyncTests()
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
        private EditExternalPrescriptionDTO CreateDefaultEditPrescriptionDTO(
            int externalPrescriptionId = 1,
            int medicalRecordHistoryId = 1,
            int userId = 1,
            DateTime? issueDate = null,
            string? note = "Updated prescription",
            List<int> medicinePrescriptionIdsToRemove = null,
            List<MedicinePrescriptionToAddDTO> medicinesToAdd = null)
        {
            return new EditExternalPrescriptionDTO
            {
                ExternalPrescriptionId = externalPrescriptionId,
                MedicalRecordHistoryId = medicalRecordHistoryId,
                UserId = userId,
                IssueDate = issueDate ?? DateTime.Now.Date,
                Note = note,
                MedicinePrescriptionIdsToRemove = medicinePrescriptionIdsToRemove ?? new List<int>(),
                MedicinesToAdd = medicinesToAdd ?? new List<MedicinePrescriptionToAddDTO>
                {
                    new MedicinePrescriptionToAddDTO { MedicineId = 1, Amount = 10, Note = "Take daily" }
                }
            };
        }

        private ExternalPrescription CreateDefaultPrescription(
            int externalPrescriptionId = 1,
            int medicalRecordHistoryId = 1,
            int userId = 1,
            DateTime? issueDate = null)
        {
            return new ExternalPrescription
            {
                ExternalPrescriptionId = externalPrescriptionId,
                MedicalRecordHistoryId = medicalRecordHistoryId,
                UserId = userId,
                IssueDate = issueDate ?? DateTime.Now.Date,
                Status = true,
                Note = "Original prescription",
                IsBhyt = false
            };
        }

        private List<Medicine> CreateDefaultValidMedicines()
        {
            return new List<Medicine>
            {
                new Medicine { MedicineId = 1, MedicineName = "Medicine A", Status = true, IsBhyt = false },
                new Medicine { MedicineId = 2, MedicineName = "Medicine B", Status = true, IsBhyt = false }
            };
        }

        private List<MedicinePrescription> CreateDefaultExistingMedicines(int externalPrescriptionId = 1)
        {
            return new List<MedicinePrescription>
            {
                new MedicinePrescription { ExternalPrescriptionId = externalPrescriptionId, MedicineId = 2, Amount = 5, Note = "Existing" }
            };
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_ValidInput_UpdatesSuccessfully()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO(
                medicinesToAdd: new List<MedicinePrescriptionToAddDTO>
                {
                    new MedicinePrescriptionToAddDTO { MedicineId = 1, Amount = 10, Note = "Take daily" }
                },
                medicinePrescriptionIdsToRemove: new List<int> { 1 });

            var prescription = CreateDefaultPrescription(dto.ExternalPrescriptionId, issueDate: DateTime.Now.Date);
            var validMedicines = CreateDefaultValidMedicines();
            var existingMedicines = CreateDefaultExistingMedicines(dto.ExternalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(validMedicines);
            _repositoryMock.Setup(r => r.GetMedicinePrescriptionsByPrescriptionIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(existingMedicines);
            _repositoryMock.Setup(r => r.UpdateExternalPrescriptionAsync(It.IsAny<ExternalPrescription>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.DeleteMedicinePrescriptionAsync(dto.ExternalPrescriptionId, It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.CreateExternalMedicinePrescriptionAsync(It.IsAny<MedicinePrescription>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.EditExternalPrescriptionForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateExternalPrescriptionAsync(It.IsAny<ExternalPrescription>()), Times.Once());
            _repositoryMock.Verify(r => r.DeleteMedicinePrescriptionAsync(dto.ExternalPrescriptionId, 1), Times.Once());
            _repositoryMock.Verify(r => r.CreateExternalMedicinePrescriptionAsync(It.IsAny<MedicinePrescription>()), Times.Once());
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_FutureIssueDate_ThrowsException()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO(issueDate: DateTime.Now.Date.AddDays(1));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditExternalPrescriptionForDoctorAsync(dto));
            Assert.Equal("Ngày phát hành không được là ngày trong tương lai!", exception.Message);
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_NonExistentPrescription_ThrowsException()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO();
            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync((ExternalPrescription)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditExternalPrescriptionForDoctorAsync(dto));
            Assert.Equal($"Không tìm thấy đơn thuốc với ID: {dto.ExternalPrescriptionId}", exception.Message);
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_NotSameDay_ThrowsException()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO();
            var prescription = CreateDefaultPrescription(dto.ExternalPrescriptionId, issueDate: DateTime.Now.Date.AddDays(-1));

            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(prescription);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditExternalPrescriptionForDoctorAsync(dto));
            Assert.Equal("Chỉ được chỉnh sửa đơn thuốc trong ngày kê đơn!", exception.Message);
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_TooManyMedicines_ThrowsException()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO(
                medicinesToAdd: Enumerable.Range(1, 10)
                    .Select(i => new MedicinePrescriptionToAddDTO { MedicineId = i, Amount = 10, Note = "Take daily" })
                    .ToList());
            var prescription = CreateDefaultPrescription(dto.ExternalPrescriptionId);
            var validMedicines = CreateDefaultValidMedicines();
            var existingMedicines = CreateDefaultExistingMedicines(dto.ExternalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(validMedicines);
            _repositoryMock.Setup(r => r.GetMedicinePrescriptionsByPrescriptionIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(existingMedicines);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditExternalPrescriptionForDoctorAsync(dto));
            Assert.Equal("Một đơn thuốc không được chứa quá 10 loại thuốc!", exception.Message);
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_DuplicateMedicineIds_ThrowsException()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO(
                issueDate: DateTime.Now.Date,
                medicinesToAdd: new List<MedicinePrescriptionToAddDTO>
                {
                    new MedicinePrescriptionToAddDTO { MedicineId = 1, Amount = 10, Note = "Take daily" },
                    new MedicinePrescriptionToAddDTO { MedicineId = 1, Amount = 5, Note = "Take twice" } // duplicate
                });
            var prescription = CreateDefaultPrescription(dto.ExternalPrescriptionId, issueDate: DateTime.Now.Date);
            var validMedicines = CreateDefaultValidMedicines();
            var existingMedicines = CreateDefaultExistingMedicines(dto.ExternalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(validMedicines);
            _repositoryMock.Setup(r => r.GetMedicinePrescriptionsByPrescriptionIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(existingMedicines);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditExternalPrescriptionForDoctorAsync(dto));
            Assert.Contains("bị trùng", exception.Message);
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_InvalidMedicineId_ThrowsException()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO(
                medicinesToAdd: new List<MedicinePrescriptionToAddDTO>
                {
                    new MedicinePrescriptionToAddDTO { MedicineId = 999, Amount = 10, Note = "Take daily" }
                });
            var prescription = CreateDefaultPrescription(dto.ExternalPrescriptionId, issueDate: DateTime.Now.Date);
            var validMedicines = CreateDefaultValidMedicines();
            var existingMedicines = CreateDefaultExistingMedicines(dto.ExternalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(validMedicines);
            _repositoryMock.Setup(r => r.GetMedicinePrescriptionsByPrescriptionIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(existingMedicines);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditExternalPrescriptionForDoctorAsync(dto));
            Assert.Equal("Không tìm thấy thuốc với ID: 999 hoặc thuốc không hoạt động!", exception.Message);
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_NonPositiveAmount_ThrowsException()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO(
                medicinesToAdd: new List<MedicinePrescriptionToAddDTO>
                {
                    new MedicinePrescriptionToAddDTO { MedicineId = 1, Amount = 0, Note = "Take daily" }
                });
            var prescription = CreateDefaultPrescription(dto.ExternalPrescriptionId, issueDate: DateTime.Now.Date);
            var validMedicines = CreateDefaultValidMedicines();
            var existingMedicines = CreateDefaultExistingMedicines(dto.ExternalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(validMedicines);
            _repositoryMock.Setup(r => r.GetMedicinePrescriptionsByPrescriptionIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(existingMedicines);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditExternalPrescriptionForDoctorAsync(dto));
            Assert.Equal("Số lượng thuốc ID: 1 phải lớn hơn 0!", exception.Message);
        }

        [Fact]
        public async Task EditExternalPrescriptionForDoctorAsync_NullMedicinesToAdd_DoesNotAddMedicines()
        {
            // Arrange
            var dto = CreateDefaultEditPrescriptionDTO(
                medicinePrescriptionIdsToRemove: new List<int> { 1 });
            dto.MedicinesToAdd.Clear();
            var prescription = CreateDefaultPrescription(dto.ExternalPrescriptionId, issueDate: DateTime.Now.Date);
            var validMedicines = CreateDefaultValidMedicines();
            var existingMedicines = CreateDefaultExistingMedicines(dto.ExternalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(validMedicines);
            _repositoryMock.Setup(r => r.GetMedicinePrescriptionsByPrescriptionIdAsync(dto.ExternalPrescriptionId))
                .ReturnsAsync(existingMedicines);
            _repositoryMock.Setup(r => r.UpdateExternalPrescriptionAsync(It.IsAny<ExternalPrescription>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.DeleteMedicinePrescriptionAsync(dto.ExternalPrescriptionId, It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.EditExternalPrescriptionForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateExternalPrescriptionAsync(It.IsAny<ExternalPrescription>()), Times.Once());
            _repositoryMock.Verify(r => r.DeleteMedicinePrescriptionAsync(dto.ExternalPrescriptionId, 1), Times.Once());
            _repositoryMock.Verify(r => r.CreateExternalMedicinePrescriptionAsync(It.IsAny<MedicinePrescription>()), Times.Never());
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}