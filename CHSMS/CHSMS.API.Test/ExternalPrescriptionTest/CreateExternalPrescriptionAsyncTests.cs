using CHSMS.API.DTOs.ExternalPrescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace CHSMS.API.Test.ExternalPrescriptionTest
{
    public class CreateExternalPrescriptionAsyncTests : IDisposable
    {
        private readonly Mock<IExternalPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _dbContextMock;
        private readonly Mock<DatabaseFacade> _databaseMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly ExternalPrescriptionService _service;

        public CreateExternalPrescriptionAsyncTests()
        {
            _repositoryMock = new Mock<IExternalPrescriptionRepository>();
            _dbContextMock = new Mock<SEP_TestContext>(new DbContextOptions<SEP_TestContext>());
            _databaseMock = new Mock<DatabaseFacade>(_dbContextMock.Object);
            _transactionMock = new Mock<IDbContextTransaction>();

            _dbContextMock.Setup(db => db.Database).Returns(_databaseMock.Object);
            _databaseMock.Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);
            _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _service = new ExternalPrescriptionService(_repositoryMock.Object, _dbContextMock.Object);
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
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never());
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
            Assert.Equal("Lỗi khi tạo đơn thuốc kê ngoài: Ngày phát hành không được là ngày trong tương lai!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
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
            Assert.Equal("Lỗi khi tạo đơn thuốc kê ngoài: Một đơn thuốc không được chứa quá 10 loại thuốc!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
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
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreateExternalPrescriptionAsync(userId, medicalRecordHistoryId, dto));
            Assert.Equal("Lỗi khi tạo đơn thuốc kê ngoài: Có thuốc bị trùng trong đơn thuốc. Vui lòng kiểm tra lại!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
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
            Assert.Equal("Lỗi khi tạo đơn thuốc kê ngoài: Không tìm thấy thuốc với ID: 1 hoặc thuốc không hoạt động!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
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
        }
    }
}