using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace CHSMS.API.Test.ExternalPrescriptionTest
{
    public class GetExternalPrescriptionDetailAsyncTests : IDisposable
    {
        private readonly Mock<IExternalPrescriptionRepository> _repositoryMock;
        private readonly SEP_TestContext _dbContext;
        private readonly ExternalPrescriptionService _service;

        public GetExternalPrescriptionDetailAsyncTests()
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
        private ExternalPrescription CreateDefaultPrescription(int externalPrescriptionId = 1)
        {
            return new ExternalPrescription
            {
                ExternalPrescriptionId = externalPrescriptionId,
                MedicalRecordHistoryId = 1,
                UserId = 1,
                IssueDate = DateTime.Now.Date,
                Status = true,
                Note = "Test prescription",
                IsBhyt = false,
                User = new User { Fullname = "Dr. Smith" },
                MedicalRecordHistory = new MedicalRecordHistory
                {
                    MedicalRecord = new MedicalRecord
                    {
                        PatientName = "John Doe",
                        Gender = "Male",
                        Dob = new DateTime(1990, 1, 1),
                        Address = "123 Main St",
                        HealthInsurance = "HI123456"
                    },
                    DiagnoseConclusion = "Flu"
                }
            };
        }

        private List<MedicinePrescription> CreateDefaultMedicinePrescriptions(int externalPrescriptionId = 1)
        {
            return new List<MedicinePrescription>
            {
                new MedicinePrescription
                {
                    ExternalPrescriptionId = externalPrescriptionId,
                    MedicineId = 1,
                    Amount = 10,
                    Note = "Take daily",
                    Medicine = new Medicine
                    {
                        MedicineId = 1,
                        MedicineName = "Medicine A",
                        DosageForm = "Tablet",
                        IsBhyt = false
                    }
                },
                new MedicinePrescription
                {
                    ExternalPrescriptionId = externalPrescriptionId,
                    MedicineId = 2,
                    Amount = 5,
                    Note = "Take twice daily",
                    Medicine = new Medicine
                    {
                        MedicineId = 2,
                        MedicineName = "Medicine B",
                        DosageForm = "Capsule",
                        IsBhyt = true
                    }
                }
            };
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_ValidId_ReturnsPrescriptionDetailDTO()
        {
            // Arrange
            int externalPrescriptionId = 1;
            var prescription = CreateDefaultPrescription(externalPrescriptionId);
            var medicinePrescriptions = CreateDefaultMedicinePrescriptions(externalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId))
                .ReturnsAsync(prescription);

            // Add medicines to in-memory database
            foreach (var mp in medicinePrescriptions)
            {
                _dbContext.MedicinePrescriptions.Add(mp);
            }
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(externalPrescriptionId, result.ExternalPrescriptionId);
            Assert.Equal(prescription.IssueDate, result.IssueDate);
            Assert.True(result.Status);
            Assert.Equal("Test prescription", result.Note);
            Assert.Equal("Dr. Smith", result.FullName);
            Assert.Equal("John Doe", result.PatientName);
            Assert.Equal("Male", result.Gender);
            Assert.Equal(new DateTime(1990, 1, 1), result.Dob);
            Assert.Equal("123 Main St", result.Address);
            Assert.Equal("HI123456", result.HealthInsurance);
            Assert.Equal("Flu", result.DiagnoseConclusion);
            Assert.False(result.IsBhyt);

            Assert.Equal(2, result.Medicines.Count);
            var firstMedicine = result.Medicines[0];
            Assert.Equal(1, firstMedicine.MedicineId);
            Assert.Equal("Medicine A", firstMedicine.MedicineName);
            Assert.Equal("Tablet", firstMedicine.DosageForm);
            Assert.Equal(10, firstMedicine.Amount);
            Assert.Equal("Take daily", firstMedicine.Note);
            Assert.False(firstMedicine.IsBhyt);

            var secondMedicine = result.Medicines[1];
            Assert.Equal(2, secondMedicine.MedicineId);
            Assert.Equal("Medicine B", secondMedicine.MedicineName);
            Assert.Equal("Capsule", secondMedicine.DosageForm);
            Assert.Equal(5, secondMedicine.Amount);
            Assert.Equal("Take twice daily", secondMedicine.Note);
            Assert.True(secondMedicine.IsBhyt);

            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId), Times.Once());
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_InvalidId_ThrowsArgumentException()
        {
            // Arrange
            int externalPrescriptionId = 0;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId));
            Assert.Equal("ExternalPrescriptionId không hợp lệ.", exception.Message);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_NonExistentPrescription_ThrowsException()
        {
            // Arrange
            int externalPrescriptionId = 1;
            _repositoryMock.Setup(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId))
                .ReturnsAsync((ExternalPrescription)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId));
            Assert.Equal("Không tìm thấy đơn thuốc ngoài", exception.Message);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId), Times.Once());
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_NullProperties_ReturnsDefaultValues()
        {
            // Arrange
            int externalPrescriptionId = 1;
            var prescription = new ExternalPrescription
            {
                ExternalPrescriptionId = externalPrescriptionId,
                MedicalRecordHistoryId = 1,
                UserId = 1,
                IssueDate = null,
                Status = null,
                Note = null,
                IsBhyt = null,
                User = null,
                MedicalRecordHistory = null
            };

            // Create a Medicine entity to satisfy the Include(mp => mp.Medicine) requirement
            var medicine = new Medicine
            {
                MedicineId = 1,
                MedicineName = null,
                DosageForm = null,
                IsBhyt = null
            };

            var medicinePrescription = new MedicinePrescription
            {
                ExternalPrescriptionId = externalPrescriptionId,
                MedicineId = 1,
                Amount = null,
                Note = null,
                Medicine = medicine  // Assign the medicine to ensure the Include works
            };

            _repositoryMock.Setup(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId))
                .ReturnsAsync(prescription);

            // Add the medicine first (for proper relationships)
            _dbContext.Medicines.Add(medicine);
            _dbContext.MedicinePrescriptions.Add(medicinePrescription);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(externalPrescriptionId, result.ExternalPrescriptionId);

            Assert.Single(result.Medicines);
            var medicineResult = result.Medicines[0];
            Assert.Equal(1, medicineResult.MedicineId);
            Assert.Equal(string.Empty, medicineResult.MedicineName);
            Assert.Equal(string.Empty, medicineResult.DosageForm);
            Assert.Equal(0, medicineResult.Amount);
            Assert.Equal(string.Empty, medicineResult.Note);
            Assert.False(medicineResult.IsBhyt);

            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId), Times.Once());
        }

        [Fact]
        public async Task GetExternalPrescriptionDetailAsync_NoMedicinePrescriptions_ReturnsEmptyMedicinesList()
        {
            // Arrange
            int externalPrescriptionId = 1;
            var prescription = CreateDefaultPrescription(externalPrescriptionId);

            _repositoryMock.Setup(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId))
                .ReturnsAsync(prescription);

            // No medicines added to the database

            // Act
            var result = await _service.GetExternalPrescriptionDetailAsync(externalPrescriptionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(externalPrescriptionId, result.ExternalPrescriptionId);
            Assert.Empty(result.Medicines);
            _repositoryMock.Verify(r => r.GetExternalPrescriptionDetailAsync(externalPrescriptionId), Times.Once());
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}