using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class EditPrescriptionForPharmacistAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<DatabaseFacade> _databaseFacadeMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly IPrescriptionService _service;

        public EditPrescriptionForPharmacistAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _databaseFacadeMock = new Mock<DatabaseFacade>(_contextMock.Object);
            _transactionMock = new Mock<IDbContextTransaction>();

            _contextMock.Setup(c => c.Database).Returns(_databaseFacadeMock.Object);
            _databaseFacadeMock.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(_transactionMock.Object);

            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_PrescriptionNotFound_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForPharmacistDTO { PrescriptionId = 1 };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1)).ReturnsAsync((Prescription)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Không tìm thấy đơn thuốc với ID: 1", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_NotSameDay_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForPharmacistDTO { PrescriptionId = 1 };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now.AddDays(-1) });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Chỉ được chỉnh sửa trạng thái đơn thuốc trong ngày phát hành đơn thuốc!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_NonExistentConsumption_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForPharmacistDTO
            {
                PrescriptionId = 1,
                MedicineConsumptionStatuses = new List<MedicineConsumptionStatusDTO>
                {
                    new MedicineConsumptionStatusDTO { MedicineConsumptionId = 1, Status = true }
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                           .ReturnsAsync((MedicineConsumption)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Không tìm thấy MedicineConsumption với ID: 1", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_ValidDispense_UpdatesSuccessfully()
        {
            // Arrange
            var dto = new EditPrescriptionForPharmacistDTO
            {
                PrescriptionId = 1,
                MedicineConsumptionStatuses = new List<MedicineConsumptionStatusDTO>
                {
                    new MedicineConsumptionStatusDTO { MedicineConsumptionId = 1, Status = true }
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                           .ReturnsAsync(new MedicineConsumption
                           {
                               MedicineConsumptionId = 1,
                               MedicineInventoryId = 1,
                               Amount = 10,
                               Status = false
                           });
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 1,
                               MedicineId = 1,
                               Quantity = 100,
                               Medicine = new Medicine { SellingPrice = 10 }
                           });
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(1))
                           .ReturnsAsync(new PrescriptionMedicineConsumption
                           {
                               PrescriptionId = 1,
                               MedicineConsumtionId = 1,
                               TotalPrice = 0
                           });
            _repositoryMock.Setup(r => r.UpdateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateMedicineInventoryAsync(It.IsAny<MedicineInventory>()))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForPharmacistAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateMedicineConsumptionAsync(It.Is<MedicineConsumption>(c => c.Status == true)), Times.Once());
            _repositoryMock.Verify(r => r.UpdateMedicineInventoryAsync(It.Is<MedicineInventory>(i => i.Quantity == 90)), Times.Once());
            _repositoryMock.Verify(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.Is<PrescriptionMedicineConsumption>(pmc => pmc.TotalPrice == 100)), Times.Once());
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.Is<Prescription>(p => p.Status == true)), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_ValidRollback_UpdatesSuccessfully()
        {
            // Arrange
            var dto = new EditPrescriptionForPharmacistDTO
            {
                PrescriptionId = 1,
                MedicineConsumptionStatuses = new List<MedicineConsumptionStatusDTO>
                {
                    new MedicineConsumptionStatusDTO { MedicineConsumptionId = 1, Status = false }
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                           .ReturnsAsync(new MedicineConsumption
                           {
                               MedicineConsumptionId = 1,
                               MedicineInventoryId = 1,
                               Amount = 10,
                               Status = true
                           });
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 1,
                               MedicineId = 1,
                               Quantity = 90
                           });
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(1))
                           .ReturnsAsync(new PrescriptionMedicineConsumption
                           {
                               PrescriptionId = 1,
                               MedicineConsumtionId = 1,
                               TotalPrice = 100
                           });
            _repositoryMock.Setup(r => r.UpdateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateMedicineInventoryAsync(It.IsAny<MedicineInventory>()))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForPharmacistAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateMedicineConsumptionAsync(It.Is<MedicineConsumption>(c => c.Status == false)), Times.Once());
            _repositoryMock.Verify(r => r.UpdateMedicineInventoryAsync(It.Is<MedicineInventory>(i => i.Quantity == 100)), Times.Once());
            _repositoryMock.Verify(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.Is<PrescriptionMedicineConsumption>(pmc => pmc.TotalPrice == 0)), Times.Once());
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.Is<Prescription>(p => p.Status == false)), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(default), Times.Once());
        }
    }
}